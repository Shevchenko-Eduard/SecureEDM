using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    private string _documentSortKey = "created";
    private bool _documentSortAscending;
    private int _documentTotal;
    private int _bodyHashVersion;

    public ObservableCollection<DocumentRow> Documents { get; } = [];
    public ObservableCollection<DocumentHistoryRow> DocumentHistory { get; } = [];

    [ObservableProperty]
    private string? _currentDocumentId;

    [ObservableProperty]
    private string _documentFormTitle = "Новый документ";

    [ObservableProperty]
    private TemplateOption? _selectedTemplate;

    [ObservableProperty]
    private string? _documentTitle;

    [ObservableProperty]
    private string? _documentAuthor;

    [ObservableProperty]
    private string? _documentBody;

    [ObservableProperty]
    private string _documentBodyHash = "—";

    [ObservableProperty]
    private string? _documentTitleError;

    [ObservableProperty]
    private string? _documentBodyError;

    [ObservableProperty]
    private string? _documentSearch;

    [ObservableProperty]
    private PageSizeOption? _selectedPageSize;

    [ObservableProperty]
    private int _documentPage = 1;

    [ObservableProperty]
    private int _documentPages = 1;

    [ObservableProperty]
    private string _documentPageLabel = "Стр. 1 из 1";

    [ObservableProperty]
    private bool _hasDocumentPager;

    [ObservableProperty]
    private bool _hasDocumentRows;

    [ObservableProperty]
    private string _emptyDocumentsText = "Документов пока нет. Заполните форму и сохраните первый документ.";

    [ObservableProperty]
    private bool _hasDocumentHistory;

    public bool HasDocumentTitleError => !string.IsNullOrEmpty(DocumentTitleError);
    public bool HasDocumentBodyError => !string.IsNullOrEmpty(DocumentBodyError);
    public bool CanPreviousDocumentPage => DocumentPage > 1;
    public bool CanNextDocumentPage => DocumentPage < DocumentPages;

    public string NumberHeader => "№" + SortMark("num");
    public string TitleHeader => "Название" + SortMark("title");
    public string AuthorHeader => "Автор" + SortMark("author");
    public string CreatedHeader => "Создан" + SortMark("created");
    public string StatusHeader => "Статус" + SortMark("status");

    partial void OnDocumentTitleErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasDocumentTitleError));

    partial void OnDocumentBodyErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasDocumentBodyError));

    partial void OnSelectedTemplateChanged(TemplateOption? value)
    {
        if (value is not { HasTemplate: true }) return;
        DocumentTitle = value.Title;
        DocumentBody = value.Body;
    }

    partial void OnDocumentBodyChanged(string? value)
    {
        var version = ++_bodyHashVersion;
        _ = UpdateBodyHashAsync(value ?? string.Empty, version);
    }

    private async Task UpdateBodyHashAsync(string text, int version)
    {
        if (string.IsNullOrEmpty(text))
        {
            if (version == _bodyHashVersion) DocumentBodyHash = "—";
            return;
        }

        try
        {
            var hash = await _service.ComputeTextHashAsync(text);
            if (version == _bodyHashVersion) DocumentBodyHash = hash;
        }
        catch
        {
            if (version == _bodyHashVersion) DocumentBodyHash = "—";
        }
    }

    partial void OnDocumentSearchChanged(string? value)
    {
        DocumentPage = 1;
        RefreshDocuments();
    }

    partial void OnSelectedPageSizeChanged(PageSizeOption? value)
    {
        DocumentPage = 1;
        RefreshDocuments();
    }

    [RelayCommand]
    private async Task SaveDocumentAsync()
    {
        if (!CanEditDocuments)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        var title = (DocumentTitle ?? string.Empty).Trim();
        var author = (DocumentAuthor ?? string.Empty).Trim();
        var body = DocumentBody ?? string.Empty;

        DocumentTitleError = string.IsNullOrWhiteSpace(title) ? "Укажите название" : null;
        DocumentBodyError = string.IsNullOrWhiteSpace(body) ? "Укажите текст документа" : null;
        if (DocumentTitleError is not null || DocumentBodyError is not null) return;

        try
        {
            if (CurrentDocumentId is { } id && _service.FindDocument(id) is not null)
            {
                var (document, changed) = await _service.UpdateDocumentAsync(id, title, author, body);
                CurrentDocumentId = document.Id;
                DocumentFormTitle = "Документ " + document.Number +
                    (document.Version > 0 ? $" · версия {document.Version}" : string.Empty);

                ShowToast(
                    changed
                        ? "Документ изменён. Ранее выданные подписи не соответствуют новому тексту."
                        : "Сохранено.",
                    changed ? "warn" : "ok");
            }
            else
            {
                var document = await _service.CreateDocumentAsync(title, author, body);
                CurrentDocumentId = document.Id;
                DocumentFormTitle = "Документ " + document.Number;
                ShowToast("Документ сохранён. Перейдите на вкладку «Подписание».", "ok");
            }

            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось сохранить документ: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private void DeleteDocument()
    {
        if (!CanEditDocuments)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        var document = _service.FindDocument(CurrentDocumentId);
        if (document is null) return;

        ShowConfirm(
            "Удалить документ?",
            $"Документ {document.Number} «{document.Title}» и связанные с ним подписи и хеши будут удалены.",
            async () =>
            {
                await _service.DeleteDocumentAsync(document.Id);
                CurrentDocumentId = null;
                DocumentTitle = string.Empty;
                DocumentAuthor = string.Empty;
                DocumentBody = string.Empty;
                SelectedTemplate = Templates[0];
                DocumentFormTitle = "Новый документ";
                DocumentBodyHash = "—";
                ShowToast("Документ удалён", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private void NewDocument()
    {
        if (!CanEditDocuments)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        _bodyHashVersion++;
        CurrentDocumentId = null;
        DocumentTitle = string.Empty;
        DocumentAuthor = string.Empty;
        DocumentBody = string.Empty;
        SelectedTemplate = Templates[0];
        DocumentFormTitle = "Новый документ";
        DocumentBodyHash = "—";
        DocumentTitleError = null;
        DocumentBodyError = null;
        RefreshDocumentHistory();
        RefreshDocuments();
    }

    [RelayCommand]
    private void OpenDocument(string? id)
    {
        var document = _service.FindDocument(id);
        if (document is null) return;

        CurrentDocumentId = document.Id;
        DocumentTitle = document.Title;
        DocumentAuthor = document.Author;
        DocumentBody = document.Content;
        SelectedTemplate = Templates[0];
        DocumentTitleError = null;
        DocumentBodyError = null;
        DocumentFormTitle = "Документ " + document.Number +
            (document.Version > 0 ? $" · версия {document.Version}" : string.Empty);
        RefreshDocumentHistory();
        RefreshDocuments();
    }

    [RelayCommand]
    private void SortDocuments(string? key)
    {
        if (key is not ("num" or "title" or "author" or "created" or "status")) return;

        if (_documentSortKey == key)
            _documentSortAscending = !_documentSortAscending;
        else
        {
            _documentSortKey = key;
            _documentSortAscending = key != "created";
        }

        RefreshDocuments();
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (DocumentPage <= 1) return;
        DocumentPage--;
        RefreshDocuments();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (DocumentPage >= DocumentPages) return;
        DocumentPage++;
        RefreshDocuments();
    }

    private void RefreshDocuments()
    {
        IEnumerable<EdoDocument> rows = _service.Documents;

        if (!string.IsNullOrWhiteSpace(DocumentSearch))
        {
            var query = DocumentSearch.Trim();
            rows = rows.Where(d =>
                d.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                d.Number.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                d.Author.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        var list = rows.ToList();
        Comparison<EdoDocument> comparison = _documentSortKey switch
        {
            "num" => (a, b) => string.Compare(a.Number, b.Number, StringComparison.CurrentCulture),
            "title" => (a, b) => string.Compare(a.Title, b.Title, StringComparison.CurrentCulture),
            "author" => (a, b) => string.Compare(a.Author, b.Author, StringComparison.CurrentCulture),
            "status" => (a, b) => string.Compare(
                _service.GetDocumentStatus(a).Text,
                _service.GetDocumentStatus(b).Text,
                StringComparison.CurrentCulture),
            _ => (a, b) => a.CreatedAt.CompareTo(b.CreatedAt),
        };

        list.Sort((a, b) => _documentSortAscending ? comparison(a, b) : comparison(b, a));

        _documentTotal = list.Count;
        var size = SelectedPageSize?.Value ?? 25;
        DocumentPages = Math.Max(1, (int)Math.Ceiling(_documentTotal / (double)size));
        if (DocumentPage > DocumentPages) DocumentPage = DocumentPages;
        if (DocumentPage < 1) DocumentPage = 1;

        Documents.Clear();
        foreach (var document in list.Skip((DocumentPage - 1) * size).Take(size))
        {
            var status = _service.GetDocumentStatus(document);
            var shortStatus = status.Kind switch
            {
                "ok" => "Подписан",
                "warn" => "Изменён",
                "bad" => "Отозвана",
                _ => "Не подписан",
            };

            Documents.Add(new DocumentRow(
                document.Id,
                document.Number,
                document.Title,
                string.IsNullOrWhiteSpace(document.Author) ? "—" : document.Author,
                FormatShortDate(document.CreatedAt),
                FormatTime(document.CreatedAt),
                ShortHash(document.Hash, 8),
                document.Hash,
                status.Kind,
                shortStatus,
                status.Text,
                document.Id == CurrentDocumentId));
        }

        HasDocumentRows = Documents.Count > 0;
        HasDocumentPager = _documentTotal > size;
        DocumentPageLabel = $"Стр. {DocumentPage} из {DocumentPages}";
        EmptyDocumentsText = string.IsNullOrWhiteSpace(DocumentSearch)
            ? "Документов пока нет. Заполните форму и сохраните первый документ."
            : "Ничего не найдено. Измените запрос.";

        OnPropertyChanged(nameof(NumberHeader));
        OnPropertyChanged(nameof(TitleHeader));
        OnPropertyChanged(nameof(AuthorHeader));
        OnPropertyChanged(nameof(CreatedHeader));
        OnPropertyChanged(nameof(StatusHeader));
        OnPropertyChanged(nameof(CanPreviousDocumentPage));
        OnPropertyChanged(nameof(CanNextDocumentPage));
    }

    private void RefreshDocumentHistory()
    {
        DocumentHistory.Clear();
        var document = _service.FindDocument(CurrentDocumentId);

        if (document?.History is { Count: > 0 })
        {
            foreach (var item in document.History.OrderByDescending(h => h.Version))
            {
                DocumentHistory.Add(new DocumentHistoryRow(
                    item.Version,
                    item.Title,
                    ShortHash(item.Hash, 10),
                    FormatShortTime(item.ArchivedAt)));
            }
        }

        HasDocumentHistory = DocumentHistory.Count > 0;
    }

    private string SortMark(string key) =>
        _documentSortKey == key ? (_documentSortAscending ? " ▲" : " ▼") : string.Empty;

    private static string FormatTime(DateTimeOffset value) =>
        value.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

    private static string FormatShortDate(DateTimeOffset value) =>
        value.ToLocalTime().ToString("dd.MM.yyyy");

    private static string FormatShortTime(DateTimeOffset value) =>
        value.ToLocalTime().ToString("dd.MM.yy HH:mm");

    private static string ShortHash(string? hash, int length = 16) =>
        string.IsNullOrEmpty(hash) ? "—" : (hash.Length > length ? hash[..length] + "…" : hash);
}

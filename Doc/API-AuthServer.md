
## Note

./ = {http/https}://{base-url}/{version-api}/

HTTP methods:

- GET - в основнгом для получения еденичных экземпляров
- QUERY - в основнгом для списка объектов или запрос с дол. параметрами
- PUT - для изменения существующих объектов
- POST - для создания объекта
- HEAD - для проверки существования объекта

## Auth

- QUERY     ./auth - возвращает id пользователя по логину
- POST      ./login - создание сессии 

## User

- GET       ./user/{user-id} - информация о пользователе
- GET       ./user/{user-id}/session-policy - политика сессий пользователя
- POST      ./user - создать пользователя
- POST      ./user/{user-id}/password - изменение пароля
- PUT       ./user/{user-id}/session-policy - изменение политики сессий
- PUT       ./user/{user-id} - обновить информацию пользователя
- QUERY     ./user/{user-id}/session - сессии пользователя
- QUERY     ./user/{user-id}/group - все группы пользователя
- QUERY     ./user/{user-id}/password/history - история хэшей паролей пользователя
- QUERY     ./user - список пользователей
- DELETE    ./user/{user-id} - мягкое удаление пользователя
- DELETE    ./user/{user-id}/perm - полное удаление пользователя

## Group

- GET       ./group/{group-id} - инфромация о группе
- GET       ./group/{group-id}/owner - владелец группы
- QUERY     ./group/{group-id}/parent - родительные группы
- QUERY     ./group/{group-id}/child - дочерние группы
- QUERY     ./group/{group-id}/user - пользователи группы
- POST      ./group/{group-id}/user - добавить пользователя в группу
- POST      ./group - создать группу
- POST      ./group/{parent-group-id}/child/{child-group-id} - добавить дочернюю группу
- DELETE    ./group/{group-id} - удадение группы
- DELETE    ./group/{group-id}/user/{user-id} - удаление пользователя из группы
- DELETE    ./group/{parent-group-id}/child/{child-group-id} - удаление дочерней группы

> `group/{group-id}/parent` и `group/{group-id}/child` по сути одно и тоже просто разных направленностей

## Session

- GET       ./session/{session-id} - информация о сессии
- GET       ./session/{session-id}/user - пользователь сессии
- GET       ./session/{session-id}/access-token - создание access токена
- PUT       ./session/{sesstion-id} - обновление информации о сессии
- QUERY     ./session/{session-id}/refresh-token - хэши refresh токенов сессии
- POST      ./session/{session-id}/refresh-token - создание токена
- DELETE    ./session/{session-id} - удаление сессии

## Refresh-token 

- GET       ./refresh-token/{token-id} - информация токена
- GET       ./refresh-token/{token-id}/user - пользователь токена
- GET       ./refresh-token/{token-id}/session - сессия токена
- QUERY     ./refresh-token - проверка на соответствие
- DELETE    ./refresh-token/{token-id} - удаление токена



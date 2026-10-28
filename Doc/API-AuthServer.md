# Entity

## Auth

| Method | URL | Description |
|---|---|---|
| `GET` | `./jwks` | JWKS-эндпоинт |
| `HEAD` | `./auth` | проверить существование пользователя по логину |
| `POST` | `./login` | создание сессии |
| `POST` | `./refresh` | обмен refresh токена на новую пару |
| `DELETE` | `./logout` | отмена сессии |

> `./logout` аналог `./session/{session-id}/rev` с другим именем

## Attest

| Method | URL | Description |
|---|---|---|
| `GET` | `./attest/{attest-id}` | имеющийся атестат |
| `QUERY` | `./attest` | имеющийся атестаты |
| `HEAD` | `./attest` | существует ли атестат |
| `DELETE` | `./attest/{attest-id}/rev` | отмена отестата |
| `POST` | `./attest/init` | первый шаг атестации |
| `PUT` | `./attest/verify` | второй шаг атестации |

## User

| Method | URL | Description |
|---|---|---|
| `GET` | `./user/{user-id}` | информация о пользователе |
| `GET` | `./user/{user-id}/session-policy` | политика сессий пользователя |
| `POST` | `./user` | создать пользователя |
| `POST` | `./user/{user-id}/password` | изменение пароля, создание нового объекта hash в историю hash |
| `PUT` | `./user/{user-id}/session-policy` | изменение политики сессий |
| `PUT` | `./user/{user-id}` | обновить информацию пользователя |
| `QUERY` | `./user/{user-id}/session` | сессии пользователя |
| `QUERY` | `./user/{user-id}/group` | все группы пользователя |
| `QUERY` | `./user/{user-id}/password/history` | история хэшей паролей пользователя |
| `QUERY` | `./user` | список пользователей |
| `DELETE` | `./user/{user-id}` | мягкое удаление пользователя |
| `DELETE` | `./user/{user-id}/hard` | полное удаление пользователя |
| `DELETE` | `./user/{user-id}/session/rev` | отмена всех сессий пользователя |
| `DELETE` | `./user/{user-id}/session/hard` | удаление всех сессий пользователя |

## Group

| Method | URL | Description |
|---|---|---|
| `GET` | `./group/{group-id}` | информация о группе |
| `GET` | `./group/{group-id}/owner` | владелец группы |
| `QUERY` | `./group` | группы |
| `QUERY` | `./group/{group-id}/parent` | родительские группы |
| `QUERY` | `./group/{group-id}/child` | дочерние группы |
| `QUERY` | `./group/{group-id}/user` | пользователи группы |
| `POST` | `./group/{group-id}/user` | добавить пользователя в группу |
| `POST` | `./group` | создать группу |
| `PUT` | `./group/{parent-group-id}/child/{child-group-id}` | добавить дочернюю группу |
| `DELETE` | `./group/{group-id}/hard` | удаление группы |
| `DELETE` | `./group/{group-id}/rev` | отмена группы |
| `DELETE` | `./group/{group-id}/user/{user-id}/hard` | удаление пользователя из группы |
| `DELETE` | `./group/{group-id}/user/{user-id}/rev` | отмена пользователя из группы |
| `DELETE` | `./group/{parent-group-id}/child/{child-group-id}/hard` | удаление дочерней группы |
| `DELETE` | `./group/{parent-group-id}/child/{child-group-id}/rev` | отмена дочерней группы |

> `./group/{group-id}/parent` и `./group/{group-id}/child` по сути одно и то же, просто разных направленностей

## Session

| Method | URL | Description |
|---|---|---|
| `GET` | `./session/{session-id}` | информация о сессии |
| `GET` | `./session/{session-id}/user` | пользователь сессии |
| `PUT` | `./session/{session-id}` | обновление информации о сессии |
| `POST` | `./session/{session-id}/access-token` | создание access токена и продление refresh токена |
| `POST` | `./session/{session-id}/refresh-token` | создание refresh токена |
| `DELETE` | `./session/{session-id}/hard` | удаление сессии |
| `DELETE` | `./session/{session-id}/rev` | отмена сессии |

## Refresh-token

| Method | URL | Description |
|---|---|---|
| `GET` | `./refresh-token/{token-id}` | информация токена |
| `GET` | `./refresh-token/{token-id}/user` | пользователь токена |
| `GET` | `./refresh-token/{token-id}/session` | сессия токена |
| `QUERY` | `./refresh-token/verify` | проверка на соответствие |
| `DELETE` | `./refresh-token/{token-id}/hard` | удаление токена |
| `DELETE` | `./refresh-token/{token-id}/rev` | отмена токена |
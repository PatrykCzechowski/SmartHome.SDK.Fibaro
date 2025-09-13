# Authentication

The client supports:
- Basic Authentication (Fibaro default)
- Bearer token (if applicable in your environment)

Configure via AddFibaroClient options:
- Username/Password for Basic
- AccessToken for Bearer

Security tips:
- Store secrets securely (User Secrets, environment variables, or Azure Key Vault)
- Never commit credentials

PIN (Scenes / protected operations):
- Niektóre sceny/operacje mogą wymagać PIN-u.
- Metody `ExecuteSceneAsync`/`ExecuteSceneSyncAsync` przyjmują `pin` i wysyłają go w nagłówku `Fibaro-User-PIN`.

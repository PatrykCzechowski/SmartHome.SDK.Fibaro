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

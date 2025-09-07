# Error handling

All HTTP errors throw FibaroApiException with:
- StatusCode
- ResponseBody (if available)

Recommended patterns:
- Catch specific status codes (403, 404) and branch
- Use cancellation tokens for timeouts
- Log ResponseBody for troubleshooting (avoid sensitive data)

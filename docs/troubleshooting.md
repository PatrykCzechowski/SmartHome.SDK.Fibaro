# Troubleshooting

Common issues:
- 401/403: Check credentials and permissions (Basic Auth default)
- 404: Verify device IDs and endpoint paths (BaseAddress should end with /api/)
- TLS/SSL: Use HTTPS; validate certificates in production
- Preview SDK: If using net10.0 preview, ensure SDK version via global.json

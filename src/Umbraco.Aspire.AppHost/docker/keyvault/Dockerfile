FROM jamesgoulddev/azure-keyvault-emulator:latest

HEALTHCHECK --interval=5m --timeout=3s \
    CMD curl --insecure -f https://localhost/token || exit 1

SHELL ["/bin/bash", "-c"]

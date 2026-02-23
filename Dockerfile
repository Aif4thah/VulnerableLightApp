FROM mcr.microsoft.com/dotnet/sdk:10.0

EXPOSE 3000

WORKDIR /app

RUN apt update && apt install -y git openssl && \
    git clone https://github.com/Aif4thah/VulnerableLightApp.git

WORKDIR /app/VulnerableLightApp

# Génération du certificat auto-signé
RUN openssl req -x509 -newkey rsa:4096 -keyout /app/key.pem -out /app/cert.pem \
    -days 365 -nodes -subj "/CN=localhost" && \
    openssl pkcs12 -export -out /app/cert.pfx \
    -inkey /app/key.pem -in /app/cert.pem -passout pass:mypassword

# Configuration HTTPS via variables d'environnement
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=mypassword

CMD ["dotnet", "run", "--url=https://0.0.0.0:3000"]
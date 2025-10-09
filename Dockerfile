# Utilise l'image officielle .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Définir l'utilisateur root
USER root

# Installer Git
RUN apt update && apt upgrade -y && apt install -y git

# Cloner le dépôt
WORKDIR /app
RUN git clone https://github.com/Aif4thah/VulnerableLightApp.git
WORKDIR /app/VulnerableLightApp

# Exposer le port
EXPOSE 3000

# Lancer l'application
CMD ["dotnet", "run", "--url=https://0.0.0.0:3000"]

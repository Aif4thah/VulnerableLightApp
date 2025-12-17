FROM debian:latest

USER root

# Mise à jour de l'image et installation des outils de base
RUN apt update && \
    apt upgrade -y && \
    apt install -y wget git

# Ajout du dépôt Microsoft pour Debian 12
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb

# Installation du SDK et du runtime .NET 10
RUN apt update && \
    apt install -y dotnet-sdk-10.0 dotnet-runtime-10.0

# Exposition du port
EXPOSE 3000

# Préparation du dossier de travail
WORKDIR /app

# Clonage du projet
RUN git clone https://github.com/Aif4thah/VulnerableLightApp.git

WORKDIR /app/VulnerableLightApp

# Lancement de l'application
CMD ["dotnet", "run", "--url=https://0.0.0.0:3000"]

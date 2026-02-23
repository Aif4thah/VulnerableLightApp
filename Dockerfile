FROM mcr.microsoft.com/dotnet/sdk:10.0

# Exposition du port
EXPOSE 3000

# Clonage du projet
RUN apt update && apt install -y git && \
    git clone https://github.com/Aif4thah/VulnerableLightApp.git

WORKDIR /VulnerableLightApp

# Lancement de l'application
CMD ["dotnet", "run", "--url=https://0.0.0.0:3000"]
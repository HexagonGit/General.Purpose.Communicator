FROM debian:latest

WORKDIR /app

RUN apt-get update && apt-get install -y wget
RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get -y install apt-transport-https
RUN apt-get update
RUN apt-get -y install dotnet-sdk-3.1
RUN apt-get -y install aspnetcore-runtime-3.1
COPY . ./

RUN apt-get update

RUN apt-get -y install nginx
RUN rm /etc/nginx/sites-available/default
COPY default /etc/nginx/sites-available/

RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "/app/out/General.Purpose.Communicator.dll"]

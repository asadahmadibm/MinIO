# MinIO
  ## With Docker
docker run -d --name minio -p 9000:9000 -p 9001:9001 -v d:/dockerdata/minio:/data -e "MINIO_ROOT_USER=asadahmadibm" -e "MINIO_ROOT_PASSWORD=123456aA@" minio/minio server /data --console-address ":9001"
  ## Minio exe in Windows
download https://dl.min.io/server/minio/release/windows-amd64/minio.exe

in Powershell goto path minio.exe then .\minio.exe server C:\minio2 --console-address :9090

in browser ENTER IP EXAMPLE http://192.168.25.59:9000

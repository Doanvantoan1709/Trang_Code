# Docker Compose Setup - Base Project với External MySQL

## Cấu hình hiện tại
- **Database**: External MySQL Server (62.146.236.71:3306)
- **API**: .NET 8 trên port 7294
- **Database Connection**: `watchers` database trên MySQL server có sẵn

## Cấu trúc files
- `docker-compose.yml` - Cấu hình chính
- `docker-compose.override.yml` - Cấu hình development 
- `.dockerignore` - Loại bỏ files không cần thiết
- `Dockerfile` - Build image cho API

## Khởi chạy project

### Lần đầu khởi chạy
```bash
# Build và chạy API service
docker-compose up --build

# Hoặc chạy ở background
docker-compose up -d --build
```

### Các lần sau
```bash
# Khởi chạy API service đã build
docker-compose up

# Hoặc ở background  
docker-compose up -d
```

## Thông tin kết nối

### API Service
- **URL**: http://localhost:7294
- **Container**: baseproject-api

### External MySQL Database  
- **Host**: 62.146.236.71
- **Port**: 3306
- **Database**: watchers
- **Username**: watchers
- **Password**: i7pnX466Ry7D4nMr

## Các lệnh hữu ích

### Quản lý containers
```bash
# Xem trạng thái containers
docker-compose ps

# Dừng tất cả services
docker-compose down

# Dừng và xóa volumes (reset database)
docker-compose down -v

# Xem logs
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f baseproject-api
docker-compose logs -f mysql-db
```

### Truy cập MySQL
```bash
# Truy cập MySQL container
docker exec -it baseproject-mysql mysql -u watchers -p

# Backup database
docker exec baseproject-mysql mysqldump -u watchers -pi7pnX466Ry7D4nMr watchers > backup.sql

# Restore database
docker exec -i baseproject-mysql mysql -u watchers -pi7pnX466Ry7D4nMr watchers < backup.sql
```

### Rebuild services
```bash
# Rebuild API service
docker-compose up --build baseproject-api

# Rebuild tất cả
docker-compose build --no-cache
```

## Cấu trúc volumes

### API Volumes
- `./BaseProjectNetCore/uploads:/app/uploads` - Upload files
- `./BaseProjectNetCore/wwwroot/uploads:/app/wwwroot/uploads` - Web uploads

### MySQL Volumes  
- `mysql-data:/var/lib/mysql` - Database persistent storage
- `./sqlbackup:/docker-entrypoint-initdb.d` - SQL init scripts

## Environment Variables

### API Environment
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:7294`
- `ConnectionStrings__MySqlConnection=server=mysql-db;port=3306;database=watchers;user=watchers;password=i7pnX466Ry7D4nMr`

### MySQL Environment
- `MYSQL_ROOT_PASSWORD=i7pnX466Ry7D4nMr`
- `MYSQL_DATABASE=watchers`
- `MYSQL_USER=watchers`
- `MYSQL_PASSWORD=i7pnX466Ry7D4nMr`

## Troubleshooting

### Database không kết nối được
1. Kiểm tra MySQL container đã chạy: `docker-compose ps`
2. Xem logs MySQL: `docker-compose logs mysql-db`
3. Kiểm tra health check: `docker inspect baseproject-mysql`

### API không khởi động
1. Xem logs API: `docker-compose logs baseproject-api`
2. Kiểm tra connection string trong appsettings.json
3. Đảm bảo MySQL đã sẵn sàng trước khi API start

### Reset toàn bộ
```bash
# Dừng và xóa tất cả
docker-compose down -v

# Xóa images (optional)
docker image prune -f

# Khởi chạy lại
docker-compose up --build
```

## Production Notes

Khi deploy production, cần thay đổi:
1. Passwords mạnh hơn
2. Cấu hình SSL/TLS
3. Backup strategy cho database
4. Resource limits cho containers
5. Health checks và monitoring 
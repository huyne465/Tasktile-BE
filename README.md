# Tasktile — Backend (.NET)

API cho Tasktile: ASP.NET Core Minimal API + EF Core + PostgreSQL, xác thực bằng JWT (Firebase Auth hoặc Sign in with Apple).

> Giai đoạn đầu app iOS chạy hoàn toàn offline, chưa cần backend. Repo này dựng sẵn để dùng khi cần sync / web / chia sẻ task.

## Cấu trúc

```
src/Tasktile.Api/
  Program.cs            # cấu hình DB, JWT, endpoints
  Auth/                 # AuthOptions, lấy user id từ claim "sub"
  Domain/               # TaskEntity
  Data/                 # AppDbContext
  Contracts/            # DTO request/response
  Endpoints/            # /api/me, /api/tasks
Dockerfile
docker-compose.yml      # api + postgres để deploy lên VPS
```

## Chạy local

Yêu cầu: .NET 10 SDK, Docker (cho Postgres).

```bash
# Postgres tạm cho dev
docker run -d --name tasktile-db -p 5432:5432 \
  -e POSTGRES_DB=tasktile -e POSTGRES_USER=tasktile -e POSTGRES_PASSWORD=tasktile_dev \
  postgres:17

# Tạo migration đầu tiên (chỉ làm một lần)
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Tasktile.Api

dotnet run --project src/Tasktile.Api
# http://localhost:5080/health
# http://localhost:5080/openapi/v1.json (môi trường Development)
```

App tự áp migration khi khởi động nếu đã có migration.

Muốn dùng .NET 8/9: đổi `TargetFramework` trong `.csproj` và version package (`8.*` / `9.*`), xoá hoặc sửa `global.json`.

## Cấu hình Auth

Chỉ khác 2 giá trị `Auth:Authority` và `Auth:Audience`:

| | Authority | Audience |
|---|---|---|
| Firebase | `https://securetoken.google.com/<projectId>` | `<projectId>` |
| Sign in with Apple | `https://appleid.apple.com` | `com.huybuu.tasktile` |

App iOS gửi ID token trong header `Authorization: Bearer <token>`. Kiểm tra nhanh bằng `GET /api/me`.

## API

| Method | Path | Mô tả |
|---|---|---|
| GET | `/health` | Không cần auth |
| GET | `/api/me` | Thông tin user từ token |
| GET | `/api/tasks` | Danh sách task của user |
| GET | `/api/tasks/{id}` | Chi tiết |
| POST | `/api/tasks` | Tạo `{ title, notes?, priority?, dueDate? }` |
| PUT | `/api/tasks/{id}` | Cập nhật toàn bộ |
| PATCH | `/api/tasks/{id}/toggle` | Đổi xong/chưa xong |
| DELETE | `/api/tasks/{id}` | Xoá |

## Deploy lên VPS

```bash
git clone https://github.com/huyne465/Tasktile-BE.git && cd Tasktile-BE
cp .env.example .env   # sửa mật khẩu + auth
# nhớ commit thư mục Migrations trước khi deploy
docker compose up -d --build
```

API chỉ mở ở `127.0.0.1:8080`. Đặt Nginx hoặc Caddy phía trước để có HTTPS (iOS yêu cầu HTTPS cho kết nối mạng mặc định).

## TODO

- Endpoint sync theo `UpdatedAt` (`GET /api/tasks?since=...`).
- Subtask, template lock screen lưu trên server.
- Rate limiting, logging có cấu trúc.

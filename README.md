# 💰 CashLink Payment API

A modern payment processing API built with .NET 8, Docker, and CI/CD automation.

## 🚀 Features

- ✅ RESTful payment API
- ✅ Docker containerization
- ✅ Automated testing
- ✅ CI/CD with GitHub Actions
- ✅ Health checks
- ✅ Swagger documentation

## 🏃 Quick Start

### Run Locally

```bash
dotnet run --project CashLink.Api
```

Visit: <http://localhost:5000/swagger>

### Run with Docker

```bash
docker-compose up -d
```

Visit: <http://localhost:5000/swagger>

## 📡 API Endpoints

### Create Payment

```bash
POST /api/payments
{
  "senderAccount": "254712345678",
  "receiverAccount": "254787654321",
  "amount": 1000,
  "currency": "KES",
  "description": "Payment for services"
}
```

### Get Payment

```bash
GET /api/payments/{id}
```

### Get All Payments

```bash
GET /api/payments
```

### Update Payment Status

```bash
PATCH /api/payments/{id}/status
"Completed"
```

## 🧪 Testing

```bash
dotnet test
```

## 🐳 Docker Commands

```bash
# Build image
docker build -t cashlink-api .

# Run container
docker run -p 5000:8080 cashlink-api

# View logs
docker logs cashlink-api

# Stop container
docker stop cashlink-api
```

## 🔄 CI/CD Pipeline

The project uses GitHub Actions for:

- ✅ Automated testing
- ✅ Docker image building
- ✅ Pushing to Docker Hub
- ✅ Security scanning

## 📊 Health Check

```bash
curl http://localhost:5000/health
```

## 🛠️ Technology Stack

- .NET 8
- Docker
- GitHub Actions
- xUnit (Testing)
- Swagger/OpenAPI

## 📝 License

MIT License

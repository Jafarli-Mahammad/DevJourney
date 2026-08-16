# Redis setup

The application selects Redis through the `ConnectionStrings:Redis` configuration key.

## Local development

Start the real Redis server in Docker:

```bash
docker compose up -d redis
```

The API running directly on the host connects to:

```text
localhost:6379
```

The development configuration already contains this local connection. The container is bound only to `127.0.0.1` and stores its data in the Docker volume `devjourney-redis-data`.

Check that it is working:

```bash
docker exec devjourney-redis redis-cli ping
```

Expected output:

```text
PONG
```

Stop it when needed:

```bash
docker compose stop redis
```

## Redis Cloud deployment

Redis Cloud uses a database connection string, not a Redis Cloud management API key, for application caching.

Configure this deployment secret/environment variable:

```text
ConnectionStrings__Redis=HOST:PORT,password=PASSWORD,ssl=True,abortConnect=False
```

Use the host, port, password, and TLS setting shown by the Redis Cloud connection wizard. The default Redis Cloud username is normally `default`; the StackExchange.Redis connection-string format above uses the default user implicitly.

For ASP.NET Core environment-variable configuration, the colon in `ConnectionStrings:Redis` becomes a double underscore: `ConnectionStrings__Redis`.

Examples:

```bash
export ConnectionStrings__Redis='redis-xxxxxxxx.c256.us-east-1-2.ec2.cloud.redislabs.com:10303,password=REPLACE_ME,ssl=True,abortConnect=False'
dotnet Devjourney/bin/Release/net10.0/Devjourney.dll
```

Never commit the password to `appsettings.json`, source control, Dockerfiles, or this document. Store it in the deployment platform's secret manager/environment settings.

## Dockerized API

If the API itself runs inside Docker on the same Compose network as Redis, use the service name rather than `localhost`:

```text
redis:6379
```

`localhost` inside the API container means the API container itself, not the Redis container.

## Fallback behavior

If `ConnectionStrings:Redis` is missing, the application registers ASP.NET Core's in-memory distributed cache. This is useful for local startup, but it is not suitable for multiple production instances because the cache is not shared between instances.

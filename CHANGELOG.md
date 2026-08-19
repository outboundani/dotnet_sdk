# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project
adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2026-08-19

Initial release. Endpoint coverage matches `@outboundiq/client` 0.2.0.

### Added

- `OutboundIQClient` with typed resources for the Assignment, Dials, Custom Dialer Integration,
  ANI Planner, NRM, and Live Feed APIs.
- Webhook signature verification via `OutboundIQWebhooks`, with typed `dial.batch` payloads.
- Automatic retries with exponential backoff and jitter. GET retries on network errors, 429, and
  5xx; POST, PUT, and DELETE retry only on 429, so writes are never duplicated.
- Per-attempt timeouts, and `CancellationToken` support on every method.
- `x-request-id` capture on every thrown exception, for support escalations.
- Multi-targeting for .NET 8, .NET 9, and .NET 10, with no runtime dependencies.
- Trim and Native AOT compatibility, backed by a source-generated `JsonSerializerContext`.

[Unreleased]: https://github.com/outboundani/dotnet_sdk/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/outboundani/dotnet_sdk/releases/tag/v0.1.0

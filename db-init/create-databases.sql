-- Postgres solo crea automáticamente la base indicada en POSTGRES_DB.
-- Este script crea la segunda base (Accounts) en el mismo contenedor/servidor,
-- manteniendo Database per Service (ADR-03/ADR-09) con un solo contenedor Docker.
CREATE DATABASE devsu_accounts;

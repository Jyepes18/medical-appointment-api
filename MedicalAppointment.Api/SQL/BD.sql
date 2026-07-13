CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE table  IF NOT EXISTS role (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role VARCHAR(50) NOT NULL UNIQUE
);

CREATE table  IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    document VARCHAR(50) NOT NULL UNIQUE,
    gender VARCHAR(20) NOT NULL,
    date_birth DATE NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    status BOOLEAN NOT NULL DEFAULT TRUE,
    role_id UUID NOT NULL,

    CONSTRAINT fk_user_role
    FOREIGN KEY (role_id)
    REFERENCES role(id)
);

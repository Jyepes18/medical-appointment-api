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

CREATE TABLE IF NOT EXISTS appointment (
                                           id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    doctor_id UUID NOT NULL,
    date TIMESTAMP NOT NULL,

    CONSTRAINT fk_appointment_user
    FOREIGN KEY (user_id)
    REFERENCES users(id),

    CONSTRAINT fk_appointment_doctor
    FOREIGN KEY (doctor_id)
    REFERENCES users(id)
    );

CREATE TABLE IF NOT EXISTS vital_sign (
                                          id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    weight DECIMAL(5,2) NOT NULL,
    height DECIMAL(5,2) NOT NULL,
    imc DECIMAL(5,2) NOT NULL,
    temperature DECIMAL(4,2) NOT NULL,
    blood_pressure VARCHAR(20) NOT NULL,
    heart_rate INT NOT NULL,
    respiratory_rate INT NOT NULL,
    oxygen_saturation INT NOT NULL
    );

CREATE TABLE IF NOT EXISTS medical_record (
                                              id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    appointment_id UUID NOT NULL,
    vital_sign_id UUID NOT NULL,
    medical_notes TEXT NOT NULL,

    CONSTRAINT fk_medical_record_user
    FOREIGN KEY (user_id)
    REFERENCES users(id),

    CONSTRAINT fk_medical_record_appointment
    FOREIGN KEY (appointment_id)
    REFERENCES appointment(id),

    CONSTRAINT fk_medical_record_vital_sign
    FOREIGN KEY (vital_sign_id)
    REFERENCES vital_sign(id)
    );

CREATE TABLE IF NOT EXISTS report (
                                      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    appointment_id UUID NOT NULL,
    url_report TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_report_appointment
    FOREIGN KEY (appointment_id)
    REFERENCES appointment(id)
    );

CREATE TABLE IF NOT EXISTS history_report (
                                              id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    url_report TEXT NOT NULL,

    CONSTRAINT fk_history_report_user
    FOREIGN KEY (user_id)
    REFERENCES users(id)
    );

CREATE DATABASE IF NOT EXISTS agrotech_db;
USE agrotech_db;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS Cultivos;
DROP TABLE IF EXISTS Parcelas;
DROP TABLE IF EXISTS Usuarios;
DROP TABLE IF EXISTS Roles;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE Roles (
    id_rol INT AUTO_INCREMENT PRIMARY KEY,
    nombre_rol VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(255),
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    id_rol INT NOT NULL,
    nombre_completo VARCHAR(150) NOT NULL,
    correo_electronico VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    estado VARCHAR(20) DEFAULT 'Activo',
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_usuario_rol
        FOREIGN KEY (id_rol)
        REFERENCES Roles(id_rol)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_estado_usuario
        CHECK (estado IN ('Activo', 'Suspendido'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Parcelas (
    id_parcela INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    ubicacion VARCHAR(255) NOT NULL,
    dimensiones_m2 DECIMAL(12,2) NOT NULL,
    registrado_por INT NOT NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_parcela_usuario
        FOREIGN KEY (registrado_por)
        REFERENCES Usuarios(id_usuario)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_dimensiones_m2
        CHECK (dimensiones_m2 >= 1.00)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Cultivos (
    id_cultivo INT AUTO_INCREMENT PRIMARY KEY,
    id_parcela INT NOT NULL,
    tipo_cultivo VARCHAR(100) NOT NULL,
    fecha_siembra DATE NOT NULL,
    fecha_estimada_cosecha DATE NOT NULL,
    estado VARCHAR(50) DEFAULT 'Activo',
    registrado_por INT NOT NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_cultivo_parcela
        FOREIGN KEY (id_parcela)
        REFERENCES Parcelas(id_parcela)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT fk_cultivo_usuario
        FOREIGN KEY (registrado_por)
        REFERENCES Usuarios(id_usuario)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT chk_fechas_cultivo
        CHECK (fecha_estimada_cosecha > fecha_siembra),
    CONSTRAINT chk_estado_cultivo
        CHECK (estado IN ('Activo', 'Cosechado', 'Perdido por Plaga'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


CREATE INDEX idx_usuario_email ON Usuarios(correo_electronico);
CREATE INDEX idx_parcela_ubicacion ON Parcelas(ubicacion);
CREATE INDEX idx_cultivo_estado ON Cultivos(estado);

INSERT INTO Roles (nombre_rol, descripcion) VALUES
('Administrador', 'Control total del sistema, administración de usuarios y configuraciones.'),
('Supervisor', 'Gestión operativa, registro de cultivos, parcelas y monitoreo de producción.'),
('Trabajador', 'Lectura de tareas agrícolas y registro básico de datos diarios.');

-- Contraseñas de prueba (hash BCrypt real, cost factor 10):
--   a.valenzuela@agrotech.cl  -> Admin123!
--   m.fuentes@agrotech.cl     -> Super123!
--   p.carrasco@agrotech.cl    -> Trabajo123!
-- Cámbialas apenas tengas el sistema arriba (usa la pantalla de Usuarios para registrar las reales).
INSERT INTO Usuarios (id_rol, nombre_completo, correo_electronico, password_hash, estado) VALUES
(1, 'Alejandro Valenzuela', 'a.valenzuela@agrotech.cl', '$2b$10$ZqC8dFq0bLXUnFC1N7o1tu0Lcsk9EH97aYJhmZ7kkX2p0xs6sGTGK', 'Activo'),
(2, 'María José Fuentes', 'm.fuentes@agrotech.cl', '$2b$10$gxmn9fB8DawXh/lvUjXCnu8HcysLiEfQ1E2yp.M08K5hCf0gsI5Ry', 'Activo'),
(3, 'Pedro Carrasco', 'p.carrasco@agrotech.cl', '$2b$10$S0Sn/lIOK2BflSeERKrMquD2BbqT4mgEJUwz0ZEaITFgZ0Q/LJx3u', 'Activo');

INSERT INTO Parcelas (nombre, ubicacion, dimensiones_m2, registrado_por) VALUES
('Invernadero Central Norte', 'Maule - Fundo Los Alerces', 25000.00, 2),
('Sector Sur Lote B', 'Ñuble - Fundo San Francisco', 48000.00, 2),
('Invernadero Hidropónico A', 'Maule - Sector Central', 12000.00, 1);

INSERT INTO Cultivos (id_parcela, tipo_cultivo, fecha_siembra, fecha_estimada_cosecha, estado, registrado_por) VALUES
(1, 'Tomate Cherry', '2026-05-15', '2026-09-15', 'Activo', 2),
(1, 'Lechuga Hidropónica', '2026-08-01', '2026-09-30', 'Activo', 2),
(2, 'Arándano O''Neal', '2025-06-10', '2026-11-20', 'Activo', 2),
(3, 'Frutilla Albión', '2026-04-01', '2026-08-15', 'Cosechado', 1);

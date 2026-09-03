-- ====================================================================================
-- PROYECTO: AgroTech SmartFields
-- DESCRIPCION: Script de inicialización y poblamiento de Base de Datos (MVP)
-- ====================================================================================

-- 1. Creación de la Base de Datos
CREATE DATABASE IF NOT EXISTS agrotech_smartfields
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE agrotech_smartfields;

-- ====================================================================================
-- 2. SEGURIDAD Y AUTENTICACIÓN (RBAC)
-- ====================================================================================

-- Tabla: Roles (Define los perfiles de acceso)
CREATE TABLE Roles (
    id_rol INT AUTO_INCREMENT PRIMARY KEY,
    nombre_rol VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(255)
);

-- Tabla: Usuarios (Credenciales, perfil y datos de contacto)
CREATE TABLE Usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    id_rol INT NOT NULL,
    nombre_completo VARCHAR(150) NOT NULL,
    correo_electronico VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL, -- Almacenamiento seguro de la contraseña
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_usuario_rol 
        FOREIGN KEY (id_rol) 
        REFERENCES Roles(id_rol) 
        ON DELETE RESTRICT 
        ON UPDATE CASCADE
);

-- ====================================================================================
-- 3. ENTIDADES DE NEGOCIO Y TRAZABILIDAD
-- ====================================================================================

-- Tabla: Parcelas (Plots)
CREATE TABLE Parcelas (
    id_parcela INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    ubicacion VARCHAR(255) NOT NULL,
    dimensiones_m2 DECIMAL(10,2) NOT NULL,
    registrado_por INT NOT NULL, -- Trazabilidad del usuario que realizó la acción
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_parcela_usuario 
        FOREIGN KEY (registrado_por) 
        REFERENCES Usuarios(id_usuario) 
        ON DELETE RESTRICT 
        ON UPDATE CASCADE
);

-- Tabla: Cultivos (Crops)
CREATE TABLE Cultivos (
    id_cultivo INT AUTO_INCREMENT PRIMARY KEY,
    id_parcela INT NOT NULL,
    tipo_cultivo VARCHAR(100) NOT NULL,
    fecha_siembra DATE NOT NULL,
    fecha_estimada_cosecha DATE NOT NULL,
    estado VARCHAR(50) DEFAULT 'Activo', -- Estados lógicos: Activo, Cosechado, Perdido
    registrado_por INT NOT NULL, -- Trazabilidad del usuario que realizó la acción
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_cultivo_parcela 
        FOREIGN KEY (id_parcela) 
        REFERENCES Parcelas(id_parcela) 
        -- RESTRICT impide eliminar una parcela si tiene cultivos asociados (Integridad)
        ON DELETE RESTRICT 
        ON UPDATE CASCADE,
    CONSTRAINT fk_cultivo_usuario 
        FOREIGN KEY (registrado_por) 
        REFERENCES Usuarios(id_usuario) 
        ON DELETE RESTRICT 
        ON UPDATE CASCADE
);

-- ====================================================================================
-- 4. OPTIMIZACIÓN (Índices para consultas eficientes)
-- ====================================================================================
-- Se crean índices en campos clave que serán muy utilizados en los "WHERE" del sistema
CREATE INDEX idx_estado_cultivo ON Cultivos(estado);
CREATE INDEX idx_fechas_cultivo ON Cultivos(fecha_siembra, fecha_estimada_cosecha);
CREATE INDEX idx_ubicacion_parcela ON Parcelas(ubicacion);

-- ====================================================================================
-- 5. INSERCIONES INICIALES (Seed Data para el MVP)
-- ====================================================================================

-- Poblar Roles
INSERT INTO Roles (nombre_rol, descripcion) VALUES
('Administrador', 'Control total del sistema, módulos sensibles y gestión de usuarios.'),
('Supervisor', 'Encargado de producción, gestiona parcelas y planifica cultivos.'),
('Trabajador', 'Acceso operativo y registro de actividades diarias de terreno.');

-- Poblar Usuarios (En un entorno real, las contraseñas estarían hasheadas con Bcrypt/Argon2)
INSERT INTO Usuarios (id_rol, nombre_completo, correo_electronico, password_hash) VALUES
(1, 'Admin Sistema', 'admin@agrotech.cl', 'hash_admin_secreto_123'),
(2, 'Carlos Producción', 'carlos.supervisor@agrotech.cl', 'hash_supervisor_secreto_123'),
(3, 'Juan Terreno', 'juan.trabajador@agrotech.cl', 'hash_trabajador_secreto_123');

-- Poblar Parcelas
INSERT INTO Parcelas (nombre, ubicacion, dimensiones_m2, registrado_por) VALUES
('Sector Norte A1', 'Maule - Fundo Las Marías', 255000.00, 1),
('Sector Sur B2', 'Ñuble - Sector Sur', 400000.00, 2);

-- Poblar Cultivos
INSERT INTO Cultivos (id_parcela, tipo_cultivo, fecha_siembra, fecha_estimada_cosecha, estado, registrado_por) VALUES
(1, 'Trigo de Invierno', '2026-05-15', '2026-11-20', 'Activo', 2),
(2, 'Maíz Dulce', '2026-08-01', '2027-01-15', 'Activo', 2);
CREATE DATABASE AnimeDB;

USE AnimeDB;

/* =============================================================
   PROYECTO: NEXUS ANIME
   AUTOR: [Tu Nombre]
   DESCRIPCION: Script de Procedimientos y Consultas para Evaluación
   ============================================================= */

-- =============================================================
-- PARTE 1: 15 PROCEDIMIENTOS ALMACENADOS (Stored Procedures)
-- =============================================================

-- 1. Registrar un nuevo usuario (Crea usuario y asigna rol/plan por defecto)
CREATE PROCEDURE sp_RegistrarUsuario
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, FechaRegistro, IsActive, RolId, PlanId)
    VALUES (@Nombre, @Email, @Password, GETDATE(), 1, 2, 1); -- 2=Usuario, 1=Aldeano(Free)
END;
GO

-- 2. Login de Usuario (Verificar credenciales)
CREATE PROCEDURE sp_LoginUsuario
    @Email NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SELECT _id, Nombre, RolId, PlanId 
    FROM Usuarios 
    WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1;
END;
GO

-- 3. Crear un Nuevo Anime
CREATE PROCEDURE sp_CrearAnime
    @Titulo NVARCHAR(200),
    @Sinopsis NVARCHAR(MAX),
    @FechaEstreno DATE,
    @PortadaUrl NVARCHAR(500),
    @EstudioId INT
AS
BEGIN
    INSERT INTO Animes (Titulo, Sinopsis, FechaEstreno, PortadaUrl, EstudioId, IsActive)
    VALUES (@Titulo, @Sinopsis, @FechaEstreno, @PortadaUrl, @EstudioId, 1);
END;
GO

-- 4. Agregar Episodio a un Anime
CREATE PROCEDURE sp_AgregarEpisodio
    @Numero INT,
    @Titulo NVARCHAR(200),
    @VideoUrl NVARCHAR(500),
    @Duracion INT,
    @AnimeId INT
AS
BEGIN
    INSERT INTO Episodios (Numero, Titulo, VideoUrl, DuracionMinutos, AnimeId)
    VALUES (@Numero, @Titulo, @VideoUrl, @Duracion, @AnimeId);
END;
GO

-- 5. Suscribir Usuario a Plan (Upgrade)
CREATE PROCEDURE sp_CambiarPlanUsuario
    @UsuarioId INT,
    @NuevoPlanId INT
AS
BEGIN
    UPDATE Usuarios SET PlanId = @NuevoPlanId WHERE _id = @UsuarioId;
END;
GO

-- 6. Registrar que un usuario vio un episodio (Historial)
CREATE PROCEDURE sp_RegistrarVisualizacion
    @UsuarioId INT,
    @EpisodioId INT
AS
BEGIN
    INSERT INTO Historial_Visualizaciones (UsuarioId, EpisodioId, FechaVisto)
    VALUES (@UsuarioId, @EpisodioId, GETDATE());
END;
GO

-- 7. Agregar Anime a Favoritos (Mi Lista)
CREATE PROCEDURE sp_AgregarFavorito
    @UsuarioId INT,
    @AnimeId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Favoritos WHERE UsuarioId = @UsuarioId AND AnimeId = @AnimeId)
    BEGIN
        INSERT INTO Favoritos (UsuarioId, AnimeId, FechaAgregado)
        VALUES (@UsuarioId, @AnimeId, GETDATE());
    END
END;
GO

-- 8. Obtener Episodios de un Anime
CREATE PROCEDURE sp_ObtenerEpisodiosPorAnime
    @AnimeId INT
AS
BEGIN
    SELECT * FROM Episodios WHERE AnimeId = @AnimeId ORDER BY Numero ASC;
END;
GO

-- 9. Buscar Animes por Texto (Buscador)
CREATE PROCEDURE sp_BuscarAnime
    @TextoBusqueda NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Animes 
    WHERE Titulo LIKE '%' + @TextoBusqueda + '%' AND IsActive = 1;
END;
GO

-- 10. Desactivar Anime (Soft Delete - No borra, solo oculta)
    UPDATE dbo.Animes
    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
    WHERE _id = @AnimeId;
END
GO

-- 11. Obtener Historial de un Usuario
CREATE PROCEDURE sp_ObtenerHistorialUsuario
    @UsuarioId INT
AS
BEGIN
    SELECT H.FechaVisto, E.Titulo as Episodio, A.Titulo as Anime
    FROM Historial_Visualizaciones H
    INNER JOIN Episodios E ON H.EpisodioId = E._id
    INNER JOIN Animes A ON E.AnimeId = A._id
    WHERE H.UsuarioId = @UsuarioId
    ORDER BY H.FechaVisto DESC;
END;
GO

-- 12. Asignar Género a un Anime
CREATE PROCEDURE sp_AsignarGeneroAnime
    @AnimeId INT,
    @GeneroId INT
AS
BEGIN
    INSERT INTO Anime_Generos (AnimeId, GeneroId) VALUES (@AnimeId, @GeneroId);
END;
GO

-- 13. Obtener Dashboard: Total de Usuarios
CREATE PROCEDURE sp_DashboardTotalUsuarios
AS
BEGIN
    SELECT COUNT(*) as Total FROM Usuarios WHERE IsActive = 1;
END;
GO

-- 14. Eliminar de Favoritos
CREATE PROCEDURE sp_EliminarFavorito
    @UsuarioId INT,
    @AnimeId INT
AS
BEGIN
    DELETE FROM Favoritos WHERE UsuarioId = @UsuarioId AND AnimeId = @AnimeId;
END;
GO

-- 15. Actualizar Perfil de Usuario
CREATE PROCEDURE sp_ActualizarPerfil
    @UsuarioId INT,
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    UPDATE Usuarios SET Nombre = @Nombre, Email = @Email WHERE _id = @UsuarioId;
END;
GO


-- =============================================================
-- PARTE 2: 15 CONSULTAS SENCILLAS (Una sola tabla)
-- =============================================================

-- 1. Ver todos los usuarios activos
SELECT * FROM Usuarios WHERE IsActive = 1;

-- 2. Ver todos los animes ordenados por fecha de estreno
SELECT * FROM Animes ORDER BY FechaEstreno DESC;

-- 3. Ver solo los animes producidos por MAPPA (Suponiendo que MAPPA es ID 1)
SELECT * FROM Animes WHERE EstudioId = 1;

-- 4. Contar cuántos episodios hay registrados en total
SELECT COUNT(*) AS TotalEpisodios FROM Episodios;

-- 5. Ver los planes que cuestan más de 0 pesos (Pagos)
SELECT * FROM Planes WHERE Precio > 0;

-- 6. Buscar usuarios que tengan correo de Gmail
SELECT * FROM Usuarios WHERE Email LIKE '%@gmail.com%';

-- 7. Ver los 5 animes agregados más recientemente (por ID)
SELECT TOP 5 * FROM Animes ORDER BY _id DESC;

-- 8. Ver lista de géneros disponibles
SELECT Nombre FROM Generos;

-- 9. Ver episodios que duren más de 24 minutos (Especiales/Películas)
SELECT * FROM Episodios WHERE DuracionMinutos > 24;

-- 10. Ver usuarios registrados en el año 2025
SELECT * FROM Usuarios WHERE YEAR(FechaRegistro) = 2025;

-- 11. Seleccionar solo Título y Sinopsis de los animes
SELECT Titulo, Sinopsis FROM Animes;

-- 12. Ver cuántos usuarios hay por cada Plan (Agrupación simple)
SELECT PlanId, COUNT(*) as CantidadUsuarios FROM Usuarios GROUP BY PlanId;

-- 13. Ver estudios activos
SELECT * FROM Estudios;

-- 14. Ver historial de visualizaciones de hoy
SELECT * FROM Historial_Visualizaciones WHERE CAST(FechaVisto AS DATE) = CAST(GETDATE() AS DATE);

-- 15. Listar administradores (RolId = 1)
SELECT * FROM Usuarios WHERE RolId = 1;


-- =============================================================
-- PARTE 3: 15 CONSULTAS MULTITABLA (JOINS)
-- =============================================================

-- 1. Usuarios con el nombre de su Rol y su Plan
SELECT U.Nombre, U.Email, R.Nombre as Rol, P.Nombre as PlanSuscripcion
FROM Usuarios U
JOIN Roles R ON U.RolId = R._id
JOIN Planes P ON U.PlanId = P._id;

-- 2. Animes con el nombre de su Estudio
SELECT A.Titulo, E.Nombre as Estudio
FROM Animes A
JOIN Estudios E ON A.EstudioId = E._id;

-- 3. Episodios con el nombre del Anime al que pertenecen
SELECT Ep.Numero, Ep.Titulo as Episodio, A.Titulo as Anime
FROM Episodios Ep
JOIN Animes A ON Ep.AnimeId = A._id;

-- 4. Ver qué géneros tiene cada Anime (Muchos a Muchos)
SELECT A.Titulo, G.Nombre as Genero
FROM Animes A
JOIN Anime_Generos AG ON A._id = AG.AnimeId
JOIN Generos G ON AG.GeneroId = G._id;

-- 5. Favoritos de cada usuario (Nombre Usuario + Título Anime)
SELECT U.Nombre, A.Titulo
FROM Favoritos F
JOIN Usuarios U ON F.UsuarioId = U._id
JOIN Animes A ON F.AnimeId = A._id;

-- 6. Top 5 Animes más agregados a favoritos (Ranking)
SELECT TOP 5 A.Titulo, COUNT(F._id) as VecesAgregado
FROM Animes A
JOIN Favoritos F ON A._id = F.AnimeId
GROUP BY A.Titulo
ORDER BY VecesAgregado DESC;

-- 7. Historial completo detallado (Quién vio qué y cuándo)
SELECT U.Nombre, A.Titulo as Anime, Ep.Numero, H.FechaVisto
FROM Historial_Visualizaciones H
JOIN Usuarios U ON H.UsuarioId = U._id
JOIN Episodios Ep ON H.EpisodioId = Ep._id
JOIN Animes A ON Ep.AnimeId = A._id;

-- 8. Ingresos totales estimados por Plan (Precio * Cantidad Usuarios)
SELECT P.Nombre, COUNT(U._id) as Usuarios, (COUNT(U._id) * P.Precio) as IngresosEstimados
FROM Planes P
JOIN Usuarios U ON P._id = U.PlanId
GROUP BY P.Nombre, P.Precio;

-- 9. Animes que NO tienen episodios registrados aún (LEFT JOIN)
SELECT A.Titulo
FROM Animes A
LEFT JOIN Episodios E ON A._id = E.AnimeId
WHERE E._id IS NULL;

-- 10. Usuarios que NO han visto nada nunca (Usuarios inactivos)
SELECT U.Nombre
FROM Usuarios U
LEFT JOIN Historial_Visualizaciones H ON U._id = H.UsuarioId
WHERE H._id IS NULL;

-- 11. Promedio de duración de episodios por Estudio
SELECT Es.Nombre, AVG(Ep.DuracionMinutos) as PromedioDuracion
FROM Estudios Es
JOIN Animes A ON Es._id = A.EstudioId
JOIN Episodios Ep ON A._id = Ep.AnimeId
GROUP BY Es.Nombre;

-- 12. Contar cuántos animes tiene cada género
SELECT G.Nombre, COUNT(AG.AnimeId) as CantidadAnimes
FROM Generos G
JOIN Anime_Generos AG ON G._id = AG.GeneroId
GROUP BY G.Nombre;

-- 13. Obtener el último episodio visto por cada usuario (Para "Continuar viendo")
SELECT U.Nombre, MAX(H.FechaVisto) as UltimaVezVisto
FROM Usuarios U
JOIN Historial_Visualizaciones H ON U._id = H.UsuarioId
GROUP BY U.Nombre;

-- 14. Animes de género 'Shonen' producidos por 'MAPPA' (Filtro complejo)
SELECT A.Titulo
FROM Animes A
JOIN Estudios E ON A.EstudioId = E._id
JOIN Anime_Generos AG ON A._id = AG.AnimeId
JOIN Generos G ON AG.GeneroId = G._id
WHERE E.Nombre = 'MAPPA' AND G.Nombre = 'Shonen';

-- 15. Lista completa de usuarios, sus planes y cuántos animes tienen en favoritos
SELECT U.Nombre, P.Nombre as Plan, COUNT(F._id) as CantidadFavoritos
FROM Usuarios U
JOIN Planes P ON U.PlanId = P._id
LEFT JOIN Favoritos F ON U._id = F.UsuarioId
GROUP BY U.Nombre, P.Nombre;

SELECT * FROM Animes;

EXEC sp_help 'Episodios';

USE [AnimeDB]
GO

/* 1. AGREGAR COLUMNA URL SI NO EXISTE
   (Según tu captura, te falta esta columna vital) */
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'VideoUrl' AND Object_ID = Object_ID(N'Episodios'))
BEGIN
    ALTER TABLE Episodios ADD VideoUrl NVARCHAR(MAX) NULL;
END
GO

/* 2. INSERTAR EL EPISODIO 1 DE KAORUKO (AnimeID = 16) */
INSERT INTO Episodios (
    Numero,           -- En tu captura se llama 'Numero', no 'NumeroEpisodio'
    Titulo, 
    Duracion, 
    AnimeId, 
    IsActive, 
    FechaRegistro,
    VideoUrl          -- Aquí va la magia
)
VALUES (
    1,                               -- Número de episodio
    'El pastel y tú',                -- Título (inventado o real del cap 1)
    24,                              -- Duración en minutos
    16,                              -- ID de Kaoru Hana (según tus datos)
    1,                               -- IsActive
    GETDATE(),                       -- Fecha actual
    'Kaoru hana wa Rin to Saku/01.mp4' 
    -- EXPLICACIÓN DE LA RUTA:
    -- /reproducir/ -> Es el prefijo que pusimos en Program.cs
    -- Kaoru hana wa Rin to Saku/ -> Es la carpeta dentro de ANIMES_HD
    -- 01.mkv -> Es el archivo
);
GO

SELECT * FROM Usuarios;
-- Crear usuario ROOT
INSERT INTO Usuarios (
    Nombre, 
    Email, 
    Password,           -- O PasswordHash según tu columna
    RolId, 
    PlanId, 
    FechaRegistro, 
    IsActive
)
VALUES (
    'adminHG',                              -- Nombre de usuario
    'ramses.hrgt17@gmail.com',              -- Email
    'RxmsesGT17',              -- Reemplaza con el hash de tu contraseña
    1,                                       -- RolId = 1 (ROOT)
    1,                                       -- PlanId = 1 (Plan básico)
    GETUTCDATE(),                           -- Fecha de registro actual (UTC)
    1                                        -- IsActive = 1 (Activo)
);

-- Verificar que se creó correctamente
SELECT _id, Nombre, Email, RolId, PlanId, FechaRegistro, IsActive
FROM Usuarios
WHERE Email = 'ramses.hrgt17@gmail.com';
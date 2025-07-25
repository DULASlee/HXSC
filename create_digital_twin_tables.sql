
-- 数字孪生系统数据库初始化脚本
-- 创建数据库（如果需要）
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'hxweb_iot')
BEGIN
    CREATE DATABASE hxweb_iot;
END
GO

USE hxweb_iot;
GO

-- 0. 创建基础表结构

-- 创建建设项目表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'construction_project')
BEGIN
    CREATE TABLE construction_project (
        project_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
        project_code NVARCHAR(50) NOT NULL UNIQUE,
        project_name NVARCHAR(200) NOT NULL,
        company_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        project_address NVARCHAR(500) NULL,
        project_manager NVARCHAR(100) NULL,
        start_date DATETIME2 NULL,
        planned_end_date DATETIME2 NULL,
        actual_end_date DATETIME2 NULL,
        status NVARCHAR(50) NOT NULL DEFAULT N'计划中',
        contract_amount DECIMAL(18,2) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL,
        is_deleted BIT NOT NULL DEFAULT 0
    );

    CREATE INDEX IX_construction_project_code ON construction_project(project_code);
    CREATE INDEX IX_construction_project_status ON construction_project(status);
END;

-- 创建设备表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'device')
BEGIN
    CREATE TABLE device (
        device_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
        device_code NVARCHAR(50) NOT NULL UNIQUE,
        device_name NVARCHAR(200) NOT NULL,
        device_type NVARCHAR(50) NOT NULL,
        model NVARCHAR(100) NULL,
        manufacturer NVARCHAR(200) NULL,
        project_id UNIQUEIDENTIFIER NULL,
        location NVARCHAR(200) NULL,
        status NVARCHAR(50) NOT NULL DEFAULT 'Offline',
        install_date DATETIME2 NULL,
        longitude FLOAT NULL,
        latitude FLOAT NULL,
        altitude FLOAT NULL,
        model_url NVARCHAR(255) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL,
        is_deleted BIT NOT NULL DEFAULT 0
    );

    CREATE INDEX IX_device_code ON device(device_code);
    CREATE INDEX IX_device_type ON device(device_type);
    CREATE INDEX IX_device_project ON device(project_id);
    CREATE INDEX IX_device_status ON device(status);
END
ELSE
BEGIN
    -- 1. 为现有设备表添加数字孪生字段（如果不存在）
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('device') AND name = 'longitude')
        ALTER TABLE device ADD longitude FLOAT NULL;
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('device') AND name = 'latitude')
        ALTER TABLE device ADD latitude FLOAT NULL;
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('device') AND name = 'altitude')
        ALTER TABLE device ADD altitude FLOAT NULL;
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('device') AND name = 'model_url')
        ALTER TABLE device ADD model_url NVARCHAR(255) NULL;
END;

-- 创建传感器数据表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'sensor_data')
BEGIN
    CREATE TABLE sensor_data (
        data_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        device_id NVARCHAR(50) NOT NULL,
        sensor_type NVARCHAR(50) NOT NULL,
        value FLOAT NULL,
        unit NVARCHAR(20) NULL,
        timestamp DATETIME2 NOT NULL,
        quality INT NOT NULL DEFAULT 100,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        is_deleted BIT NOT NULL DEFAULT 0
    );

    CREATE INDEX IX_sensor_data_device ON sensor_data(device_id);
    CREATE INDEX IX_sensor_data_type ON sensor_data(sensor_type);
    CREATE INDEX IX_sensor_data_timestamp ON sensor_data(timestamp);
    CREATE INDEX IX_sensor_data_device_timestamp ON sensor_data(device_id, timestamp);
END;

-- 2. 创建数字孪生报警表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'dt_alerts')
BEGIN
    CREATE TABLE dt_alerts (
        AlertId BIGINT IDENTITY(1,1) PRIMARY KEY,
        DeviceId NVARCHAR(50) NOT NULL,
        Timestamp DATETIME2 NOT NULL,
        Type NVARCHAR(50) NOT NULL,
        Value FLOAT NULL,
        Status INT NOT NULL DEFAULT 0, -- 0=New, 1=Acknowledged, 2=Resolved, 3=Closed
        Message NVARCHAR(500) NULL,
        Severity NVARCHAR(20) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );

    -- 创建索引
    CREATE INDEX IX_dt_alerts_DeviceId ON dt_alerts(DeviceId);
    CREATE INDEX IX_dt_alerts_Type ON dt_alerts(Type);
    CREATE INDEX IX_dt_alerts_Status ON dt_alerts(Status);
    CREATE INDEX IX_dt_alerts_Timestamp ON dt_alerts(Timestamp);
    CREATE INDEX IX_dt_alerts_Severity ON dt_alerts(Severity);
END;

-- 3. 创建环境数据表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'dt_environment_data')
BEGIN
    CREATE TABLE dt_environment_data (
        DataId BIGINT IDENTITY(1,1) PRIMARY KEY,
        ProjectId NVARCHAR(50) NOT NULL,
        MonitoringPoint NVARCHAR(100) NOT NULL,
        Timestamp DATETIME2 NOT NULL,
        PM25 FLOAT NULL,
        PM10 FLOAT NULL,
        TSP FLOAT NULL,
        NoiseLevel FLOAT NULL,
        Temperature FLOAT NULL,
        Humidity FLOAT NULL,
        WindSpeed FLOAT NULL,
        WindDirection NVARCHAR(20) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );

    -- 创建索引
    CREATE INDEX IX_dt_environment_data_ProjectId ON dt_environment_data(ProjectId);
    CREATE INDEX IX_dt_environment_data_MonitoringPoint ON dt_environment_data(MonitoringPoint);
    CREATE INDEX IX_dt_environment_data_Timestamp ON dt_environment_data(Timestamp);
    CREATE INDEX IX_dt_environment_data_ProjectId_Timestamp ON dt_environment_data(ProjectId, Timestamp);
END;

-- 4. 插入测试项目数据
IF NOT EXISTS (SELECT * FROM construction_project WHERE project_code = 'PROJ-001')
BEGIN
    -- 插入项目时不指定ID，让数据库自动生成连续GUID
    INSERT INTO construction_project (project_code, project_name, project_address, project_manager, start_date, planned_end_date, status, contract_amount, created_at)
    VALUES 
    ('PROJ-001', N'智慧工地示范项目A区', N'北京市朝阳区建国路88号', N'张工程师', DATEADD(month, -6, GETDATE()), DATEADD(month, 6, GETDATE()), N'在建', 150000000.00, GETUTCDATE()),
    ('PROJ-002', N'B地块综合体项目', N'北京市朝阳区东三环北路38号', N'李工程师', DATEADD(month, -8, GETDATE()), DATEADD(month, 4, GETDATE()), N'在建', 220000000.00, GETUTCDATE());
END;

-- 5. 插入测试设备数据
DECLARE @ProjectId1 UNIQUEIDENTIFIER = (SELECT TOP 1 project_id FROM construction_project WHERE project_code = 'PROJ-001');
DECLARE @ProjectId2 UNIQUEIDENTIFIER = (SELECT TOP 1 project_id FROM construction_project WHERE project_code = 'PROJ-002');

-- 为项目1添加设备
DECLARE @i INT = 1;
WHILE @i <= 50
BEGIN
    IF NOT EXISTS (SELECT * FROM device WHERE device_code = 'PROJ-001-DEV-' + FORMAT(@i, '000'))
    BEGIN
        -- 不指定device_id，让数据库自动生成连续GUID
        INSERT INTO device (device_code, device_name, device_type, model, manufacturer, project_id, location, status, longitude, latitude, altitude, model_url, created_at)
        VALUES (
            'PROJ-001-DEV-' + FORMAT(@i, '000'),
            CAST(@i AS NVARCHAR) + N'号设备',
            CASE (@i % 5) 
                WHEN 0 THEN 'TowerCrane'
                WHEN 1 THEN 'Elevator' 
                WHEN 2 THEN 'Camera'
                WHEN 3 THEN 'EnvironmentalSensor'
                ELSE 'SafetySensor'
            END,
            'Model-' + CAST(CAST(RAND() * 9000 + 1000 AS INT) AS NVARCHAR),
            N'智慧工地设备有限公司',
            @ProjectId1,
            N'施工区域' + CAST(@i AS NVARCHAR),
            'Online',
            116.395000 + (RAND() * 0.005),
            39.907000 + (RAND() * 0.005),
            RAND() * 100,
            '/models/device.glb',
            GETUTCDATE()
        );
    END;
    SET @i = @i + 1;
END;

-- 为项目2添加设备
SET @i = 1;
WHILE @i <= 50
BEGIN
    IF NOT EXISTS (SELECT * FROM device WHERE device_code = 'PROJ-002-DEV-' + FORMAT(@i, '000'))
    BEGIN
        -- 不指定device_id，让数据库自动生成连续GUID
        INSERT INTO device (device_code, device_name, device_type, model, manufacturer, project_id, location, status, longitude, latitude, altitude, model_url, created_at)
        VALUES (
            'PROJ-002-DEV-' + FORMAT(@i, '000'),
            CAST(@i AS NVARCHAR) + N'号设备',
            CASE (@i % 5) 
                WHEN 0 THEN 'TowerCrane'
                WHEN 1 THEN 'Elevator' 
                WHEN 2 THEN 'Camera'
                WHEN 3 THEN 'EnvironmentalSensor'
                ELSE 'SafetySensor'
            END,
            'Model-' + CAST(CAST(RAND() * 9000 + 1000 AS INT) AS NVARCHAR),
            N'智慧工地设备有限公司',
            @ProjectId2,
            N'施工区域' + CAST(@i AS NVARCHAR),
            'Online',
            116.398000 + (RAND() * 0.005),
            39.909000 + (RAND() * 0.005),
            RAND() * 100,
            '/models/device.glb',
            GETUTCDATE()
        );
    END;
    SET @i = @i + 1;
END;

-- 6. 插入传感器测试数据（7天数据，每小时一条）
DECLARE @DeviceCode NVARCHAR(50);
DECLARE @StartTime DATETIME2 = DATEADD(day, -7, GETUTCDATE());
DECLARE @EndTime DATETIME2 = GETUTCDATE();
DECLARE @CurrentTime DATETIME2;

DECLARE device_cursor CURSOR FOR 
SELECT device_code FROM device WHERE device_code LIKE 'PROJ-%';

OPEN device_cursor;
FETCH NEXT FROM device_cursor INTO @DeviceCode;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @CurrentTime = @StartTime;
    
    WHILE @CurrentTime <= @EndTime
    BEGIN
        -- 插入传感器数据
        INSERT INTO sensor_data (device_id, sensor_type, value, unit, timestamp, quality, is_deleted, created_at)
        VALUES 
        (@DeviceCode, 'Temperature', 20 + (RAND() * 15), '°C', @CurrentTime, 100, 0, @CurrentTime),
        (@DeviceCode, 'Humidity', 40 + (RAND() * 40), '%', @CurrentTime, 100, 0, @CurrentTime),
        (@DeviceCode, 'Load', 1000 + (RAND() * 4000), 'kg', @CurrentTime, 100, 0, @CurrentTime);
        
        -- 5%概率生成报警
        IF RAND() < 0.05
        BEGIN
            INSERT INTO dt_alerts (DeviceId, Timestamp, Type, Value, Status, Message, Severity)
            VALUES (
                @DeviceCode, 
                @CurrentTime, 
                CASE CAST(RAND() * 4 AS INT) 
                    WHEN 0 THEN 'OverLoad'
                    WHEN 1 THEN 'HighTemperature'
                    WHEN 2 THEN 'NoiseExceeded'
                    ELSE 'SystemAlert'
                END,
                RAND() * 100,
                0, -- New
                N'自动生成的测试报警',
                CASE CAST(RAND() * 3 AS INT)
                    WHEN 0 THEN 'Low'
                    WHEN 1 THEN 'Medium'
                    ELSE 'High'
                END
            );
        END;
        
        SET @CurrentTime = DATEADD(hour, 1, @CurrentTime); -- 每小时一条数据
    END;
    
    FETCH NEXT FROM device_cursor INTO @DeviceCode;
END;

CLOSE device_cursor;
DEALLOCATE device_cursor;

-- 7. 插入环境监测数据
DECLARE @ProjectCode NVARCHAR(50);
DECLARE project_cursor CURSOR FOR 
SELECT project_code FROM construction_project WHERE project_code LIKE 'PROJ-%';

OPEN project_cursor;
FETCH NEXT FROM project_cursor INTO @ProjectCode;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @CurrentTime = DATEADD(day, -7, GETUTCDATE());
    
    WHILE @CurrentTime <= @EndTime
    BEGIN
        -- 为每个项目的3个监测点插入数据
        INSERT INTO dt_environment_data (ProjectId, MonitoringPoint, Timestamp, PM25, PM10, TSP, NoiseLevel, Temperature, Humidity, WindSpeed, WindDirection)
        VALUES 
        (@ProjectCode, N'东门监测点', @CurrentTime, 20 + (RAND() * 60), 40 + (RAND() * 80), 100 + (RAND() * 200), 45 + (RAND() * 30), 15 + (RAND() * 20), 40 + (RAND() * 40), RAND() * 15, N'北风'),
        (@ProjectCode, N'西门监测点', @CurrentTime, 25 + (RAND() * 55), 45 + (RAND() * 75), 110 + (RAND() * 190), 50 + (RAND() * 25), 18 + (RAND() * 17), 45 + (RAND() * 35), RAND() * 12, N'南风'),
        (@ProjectCode, N'中心监测点', @CurrentTime, 30 + (RAND() * 50), 50 + (RAND() * 70), 120 + (RAND() * 180), 55 + (RAND() * 20), 20 + (RAND() * 15), 50 + (RAND() * 30), RAND() * 10, N'东风');
        
        SET @CurrentTime = DATEADD(hour, 1, @CurrentTime);
    END;
    
    FETCH NEXT FROM project_cursor INTO @ProjectCode;
END;

CLOSE project_cursor;
DEALLOCATE project_cursor;

-- 显示执行结果统计
PRINT N'数字孪生测试数据生成完成！';
PRINT N'- 已创建2个测试项目';
PRINT N'- 已创建100个测试设备（每项目50个）';
PRINT N'- 已生成7天传感器历史数据（1小时间隔）';
PRINT N'- 已生成约5%的报警记录';
PRINT N'- 已生成7天环境监测数据（1小时间隔）';

-- 显示数据统计
SELECT '项目数' AS 统计项, COUNT(*) AS 数量 FROM construction_project WHERE is_deleted = 0
UNION ALL
SELECT '设备数', COUNT(*) FROM device WHERE is_deleted = 0
UNION ALL
SELECT '传感器数据', COUNT(*) FROM sensor_data WHERE is_deleted = 0
UNION ALL
SELECT '报警数', COUNT(*) FROM dt_alerts WHERE IsDeleted = 0
UNION ALL
SELECT '环境数据', COUNT(*) FROM dt_environment_data WHERE IsDeleted = 0;

-- 查看生成的连续GUID示例
PRINT N'';
PRINT N'连续GUID示例：';
SELECT TOP 5 
    project_id,
    project_code,
    project_name
FROM construction_project
ORDER BY project_id;

SELECT TOP 5 
    device_id,
    device_code,
    device_name
FROM device
ORDER BY device_id;

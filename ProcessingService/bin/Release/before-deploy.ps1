

NET STOP "IQI - ExportDirector $env:ServiceEnvironmentID"
NET STOP "IQI - IncrementalService $env:ServiceEnvironmentID"
NET STOP "IQI - CoreWarningDirector $env:ServiceEnvironmentID"
NET STOP "IQI - EmailNotificationDirector $env:ServiceEnvironmentID"
NET STOP "IQI - SMSNotificationDirector $env:ServiceEnvironmentID"
NET STOP "IQI - CubeJobService $env:ServiceEnvironmentID"

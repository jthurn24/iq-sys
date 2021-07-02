

NET STOP "IQI - ExportDirector $env:ServiceEnvironmentID"
NET STOP "IQI - IncrementalService $env:ServiceEnvironmentID"
NET STOP "IQI - CoreWarningDirector $env:ServiceEnvironmentID"
NET STOP "IQI - EmailNotificationDirector $env:ServiceEnvironmentID"
NET STOP "IQI - SMSNotificationDirector $env:ServiceEnvironmentID"
NET STOP "IQI - CubeJobService $env:ServiceEnvironmentID"


Copy-Item -force -recurse *  $env:APPLICATION_PATH

NET START "IQI - ExportDirector $env:ServiceEnvironmentID"
NET START "IQI - IncrementalService $env:ServiceEnvironmentID"
NET START "IQI - CoreWarningDirector $env:ServiceEnvironmentID"
NET START "IQI - EmailNotificationDirector $env:ServiceEnvironmentID"
NET START "IQI - SMSNotificationDirector $env:ServiceEnvironmentID"
NET START "IQI - CubeJobService $env:ServiceEnvironmentID"

$command = 'cmd.exe /C ' + $env:APPLICATION_PATH + '/Iqi.Intuition.Console.exe InstallScripts ' + $env:APPLICATION_PATH + '/Scripts/'

Invoke-Expression -Command:$command

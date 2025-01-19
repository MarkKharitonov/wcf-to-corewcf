param([Switch]$wcf, [Switch]$api)

$ErrorActionPreference='STOP'
$IISExpress = "c:\Program Files\IIS Express\iisexpress.exe" 

function Run($Site)
{
    $ArgList = '/config:"C:\work\wcf-to-corewcf\applicationhost.config" /site:' + $Site + ' /apppool:"' + $Site + ' AppPool"'
    $LogFile = "c:\temp\iisexpress-$Site.log"
    Remove-Item -Force $LogFile -EA SilentlyContinue
    $IISExpressPID = (Start-Process $IISExpress $ArgList -PassThru -RedirectStandardOutput $LogFile -WindowStyle Hidden).Id
    Write-Host -ForegroundColor Cyan "$Site $IISExpressPID , $LogFile"
}

if ($wcf)
{
    Run "CoreWCF.SampleService"
    start http://localhost:5000/Services/AuthService.svc
}
if ($api)
{
    Run "api"
    start http://localhost:54582/weatherforecast
}
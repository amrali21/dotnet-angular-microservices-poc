$nginxRoot = "C:\nginx-1.27.3"
$nginxConfig = "$nginxRoot\conf\nginx.conf"
$buildLocation = Resolve-Path "dist/test-angular/browser"

$liveTestConfigs = (get-content -raw $nginxConfig) 

# get server blocks from nginx config (live and test)
$liveTestServers = ($liveTestConfigs | select-string -AllMatches -pattern '#server_start([\s\S]*?)#server_end').Matches

#get test block
$testblock = ($liveTestServers|where {$_ -match "test"})

#get test server (blue or green)
$testServer = ($testblock | select-string  -AllMatches -pattern '\b(green|blue)\b').Matches[0]

$testSite = "${nginxRoot}\mysites\$($testServer)"

Set-Location $nginxRoot
.\nginx -s reload

#copy files from build location to test server location
Write-Host "Copying from $buildLocation to $testSite"
Copy-Item -Path "$buildLocation\*" -Destination "$testSite" -Recurse -Force

Write-Host "Deployed to test successfully"
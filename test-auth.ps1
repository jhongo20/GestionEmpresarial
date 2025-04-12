$loginUrl = "http://localhost:5000/api/auth/login"
$testUrl = "http://localhost:5000/api/auth/test"
$protectedUrl = "http://localhost:5000/api/auth/protected"

# Verificar que la API esté en funcionamiento
try {
    Write-Host "Verificando que la API esté en funcionamiento..."
    $testResponse = Invoke-RestMethod -Uri $testUrl -Method Get
    Write-Host "API en funcionamiento: $($testResponse.Message)" -ForegroundColor Green
} catch {
    Write-Host "Error al conectar con la API: $_" -ForegroundColor Red
    exit
}

# Intentar autenticarse
$loginBody = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

try {
    Write-Host "Intentando autenticarse..."
    $loginResponse = Invoke-RestMethod -Uri $loginUrl -Method Post -Body $loginBody -ContentType "application/json"
    Write-Host "Autenticación exitosa para el usuario: $($loginResponse.Username)" -ForegroundColor Green
    Write-Host "Token JWT: $($loginResponse.Token)"
    
    # Guardar el token para usarlo en solicitudes protegidas
    $token = $loginResponse.Token
    
    # Intentar acceder a un endpoint protegido
    Write-Host "Intentando acceder a un endpoint protegido..."
    $headers = @{
        Authorization = "Bearer $token"
    }
    
    $protectedResponse = Invoke-RestMethod -Uri $protectedUrl -Method Get -Headers $headers
    Write-Host "Acceso exitoso al endpoint protegido: $($protectedResponse.Message)" -ForegroundColor Green
    Write-Host "Usuario autenticado: $($protectedResponse.User)"
    
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

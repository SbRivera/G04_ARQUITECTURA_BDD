<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>
<html>
<head>
    <title>Eureka Bank - Web Service SOAP</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 50px; }
        .container { max-width: 600px; margin: 0 auto; text-align: center; }
        .service-link { 
    display: inline-block; 
  padding: 10px 20px; 
     background-color: #007cba; 
   color: white; 
            text-decoration: none; 
   border-radius: 5px; 
      margin: 10px;
      }
   .service-link:hover { background-color: #005a87; }
    </style>
</head>
<body>
    <div class="container">
     <h1>Eureka Bank - Servicios Web SOAP</h1>
        <p>Bienvenido al sistema de servicios web de Eureka Bank</p>
        
     <h3>Servicios Disponibles:</h3>
        <a href="ec.edu.monster.ws/WSEureka.asmx" class="service-link">
            EurekaService - Servicios Bancarios
        </a>
 
        <h3>Métodos Disponibles:</h3>
        <ul style="text-align: left;">
        <li><strong>ValidarIngreso</strong> - Validar credenciales de usuario</li>
            <li><strong>TraerMovimientos</strong> - Obtener movimientos de cuenta</li>
       <li><strong>RegistrarDeposito</strong> - Registrar un depósito</li>
  <li><strong>RegistrarRetiro</strong> - Registrar un retiro</li>
     <li><strong>RegistrarTransferencia</strong> - Registrar una transferencia</li>
     </ul>
    </div>
</body>
</html>
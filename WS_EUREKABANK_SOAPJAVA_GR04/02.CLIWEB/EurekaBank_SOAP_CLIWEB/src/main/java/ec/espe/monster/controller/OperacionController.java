package ec.espe.monster.controller;

import ec.espe.monster.service.EurekaWebClient;
import jakarta.servlet.*;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.*;

import java.io.IOException;

@WebServlet(name = "OperacionController", urlPatterns = {"/operacion"})
public class OperacionController extends HttpServlet {

    @Override
    protected void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        HttpSession session = request.getSession(false);
        if (session == null || session.getAttribute("usuario") == null) {
            response.sendRedirect(request.getContextPath() + "/index.jsp");
            return;
        }

        String action = request.getParameter("action");
        String mensaje;

        try {
            switch (action) {
                case "deposito": {
                    String cuenta = request.getParameter("cuenta");
                    double importe = Double.parseDouble(request.getParameter("importe"));
                    int estado = EurekaWebClient.regDeposito(cuenta, importe);
                    mensaje = (estado == 1) ? "Depósito realizado correctamente." : "Error al realizar el depósito.";
                    break;
                }
                case "retiro": {
                    String cuenta = request.getParameter("cuenta");
                    double importe = Double.parseDouble(request.getParameter("importe"));
                    int estado = EurekaWebClient.regRetiro(cuenta, importe);
                    mensaje = (estado == 1) ? "Retiro realizado correctamente." : "Error al realizar el retiro.";
                    break;
                }
                case "transferencia": {
                    String cuentaOrigen = request.getParameter("cuentaOrigen");
                    String cuentaDestino = request.getParameter("cuentaDestino");
                    double importe = Double.parseDouble(request.getParameter("importe"));
                    int estado = EurekaWebClient.regTransferencia(cuentaOrigen, cuentaDestino, importe);
                    mensaje = (estado == 1) ? "Transferencia realizada correctamente."
                                            : "Error al realizar la transferencia.";
                    break;
                }
                default:
                    mensaje = "Operación no reconocida.";
            }
        } catch (Exception ex) {
            mensaje = "Error en la operación: " + ex.getMessage();
        }

        request.setAttribute("mensaje", mensaje);
        RequestDispatcher rd = request.getRequestDispatcher("/WEB-INF/views/mensaje.jsp");
        rd.forward(request, response);
    }
}

package ec.edu.monster.modelo;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import org.json.JSONArray;
import org.json.JSONObject;

public class CliCon_Modelo {

    private static final String BASE_URL = "http://localhost:8080/WSEurekaBank_Restfull_Java_G4/resources/corebancario";

    public String registrarDeposito(String cuenta, double importe) throws Exception {
        String endpoint = BASE_URL + "/deposito?cuenta=" + cuenta + "&importe=" + importe;
        URL url = new URL(endpoint);
        HttpURLConnection con = (HttpURLConnection) url.openConnection();
        con.setRequestMethod("POST");
        con.setRequestProperty("Content-Type", "application/json");
        con.setDoOutput(true);

        int responseCode = con.getResponseCode();
        if (responseCode == 200) {
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream(), StandardCharsets.UTF_8));
            String response = in.readLine();
            in.close();
            JSONObject jsonResponse = new JSONObject(response);
            return jsonResponse.getInt("estado") == 1 ? "\033[32mDepósito registrado exitosamente.\033[0m" : "\033[31mError al registrar depósito.\033[0m";
        } else {
            throw new RuntimeException("Error al consumir el servicio. Código: " + responseCode);
        }
    }
    
    public String registrarRetiro(String cuenta, double importe) throws Exception {
        String endpoint = BASE_URL + "/retiro?cuenta=" + cuenta + "&importe=" + importe;
        URL url = new URL(endpoint);
        HttpURLConnection con = (HttpURLConnection) url.openConnection();
        con.setRequestMethod("POST");
        con.setRequestProperty("Content-Type", "application/json");
        con.setDoOutput(true);

        int responseCode = con.getResponseCode();
        if (responseCode == 200) {
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream(), StandardCharsets.UTF_8));
            String response = in.readLine();
            in.close();
            JSONObject jsonResponse = new JSONObject(response);
            return jsonResponse.getInt("estado") == 1 ? "\033[32mRetiro registrado exitosamente.\033[0m" : "\033[31mError al registrar retiro.\033[0m";
        } else {
            throw new RuntimeException("Error al consumir el servicio. Código: " + responseCode);
        }
    }

    public String registrarTransferencia(String cuentaOrigen, String cuentaDestino, double importe) throws Exception {
        String endpoint = BASE_URL + "/transferencia?cuentaOrigen=" + cuentaOrigen + "&cuentaDestino=" + cuentaDestino + "&importe=" + importe;
        URL url = new URL(endpoint);
        HttpURLConnection con = (HttpURLConnection) url.openConnection();
        con.setRequestMethod("POST");
        con.setRequestProperty("Content-Type", "application/json");
        con.setDoOutput(true);

        int responseCode = con.getResponseCode();
        if (responseCode == 200) {
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream(), StandardCharsets.UTF_8));
            String response = in.readLine();
            in.close();
            JSONObject jsonResponse = new JSONObject(response);
            return jsonResponse.getInt("estado") == 1 ? "\033[32mTransferencia registrada exitosamente.\033[0m" : "\033[31mError al registrar transferencia.\033[0m";
        } else {
            throw new RuntimeException("Error al consumir el servicio. Código: " + responseCode);
        }
    }


    public JSONArray obtenerMovimientos(String cuenta) throws Exception {
        String endpoint = BASE_URL + "/movimientos/" + cuenta;
        URL url = new URL(endpoint);
        HttpURLConnection con = (HttpURLConnection) url.openConnection();
        con.setRequestMethod("GET");
        con.setRequestProperty("Accept", "application/json");

        int responseCode = con.getResponseCode();
        if (responseCode == 200) {
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream(), StandardCharsets.UTF_8));
            StringBuilder response = new StringBuilder();
            String line;
            while ((line = in.readLine()) != null) {
                response.append(line);
            }
            in.close();
            return new JSONArray(response.toString());
        } else {
            throw new RuntimeException("Error al consumir el servicio. Código: " + responseCode);
        }
    }
}

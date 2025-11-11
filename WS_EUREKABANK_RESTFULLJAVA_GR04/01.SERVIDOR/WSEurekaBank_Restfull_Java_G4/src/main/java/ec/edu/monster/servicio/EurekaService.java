package ec.edu.monster.servicio;

import ec.edu.monster.db.AccesoDB; 
import ec.edu.monster.modelo.Movimiento;
import java.sql.Connection; 
import java.sql.PreparedStatement; 
import java.sql.ResultSet; 
import java.sql.SQLException; 
import java.util.ArrayList; 
import java.util.List; 

public class EurekaService {
    
    public List<Movimiento> leerMovimientos(String cuenta) {
        Connection cn = null;
        List<Movimiento> lista = new ArrayList<Movimiento>();
        String sql = "SELECT \n"
                + " m.chr_cuencodigo cuenta, \n"
                + " m.int_movinumero nromov, \n"
                + " m.dtt_movifecha fecha, \n"
                + " t.vch_tipodescripcion tipo, \n"
                + " t.vch_tipoaccion accion, \n"
                + " m.dec_moviimporte importe \n"
                + "FROM tipomovimiento t INNER JOIN movimiento m \n"
                + "ON t.chr_tipocodigo = m.chr_tipocodigo \n"
                + "WHERE m.chr_cuencodigo = ?"
                + "order by 2";

        try {
            cn = AccesoDB.getConnection();
            PreparedStatement pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            ResultSet rs = pstm.executeQuery();

            while (rs.next()) {
                Movimiento rec = new Movimiento();
                rec.setCuenta(rs.getString("cuenta"));
                rec.setNromov(rs.getInt("nromov"));
                rec.setFecha(rs.getDate("fecha"));
                rec.setTipo(rs.getString("tipo"));
                rec.setAccion(rs.getString("accion"));
                rec.setImporte(rs.getDouble("importe"));

                lista.add(rec);
            }
            rs.close();

        } catch (SQLException e) {
            throw new RuntimeException(e.getMessage());
        } finally {
            try {
                cn.close();
            } catch (Exception e) {
            }
        }
        return lista;
    }

    public void registrarDeposito(String cuenta, double importe, String codEmp) {
        Connection cn = null;
        try {
            // Obtener la conexión
            cn = AccesoDB.getConnection();
            // Habilitar la transacción
            cn.setAutoCommit(false);
            
            // Paso 1: Leer datos de la cuenta
            String sql = "SELECT dec_cuensaldo, int_cuencontmov "
                       + "FROM cuenta "
                       + "WHERE chr_cuencodigo = ? AND vch_cuenestado = 'ACTIVO' "
                       + "FOR UPDATE";
            PreparedStatement pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            ResultSet rs = pstm.executeQuery();
            if (!rs.next()) {
                throw new SQLException("ERROR, cuenta no existe, o no está activa");
            }
            double saldo = rs.getDouble("dec_cuensaldo");
            int cont = rs.getInt("int_cuencontmov");
            rs.close();
            pstm.close();

            // Paso 2: Actualizar la cuenta
            saldo += importe;
            cont++;
            sql = "UPDATE cuenta SET dec_cuensaldo = ?, int_cuencontmov = ? "
            + "WHERE chr_cuencodigo = ? AND vch_cuenestado = 'ACTIVO'";
            pstm = cn.prepareStatement(sql);
            pstm.setDouble(1, saldo);
            pstm.setInt(2, cont);
            pstm.setString(3, cuenta);
            pstm.executeUpdate();
            pstm.close();

            // Paso 3: Registrar movimiento
            sql = "insert into movimiento(chr_cuencodigo,"
                    + "int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,"
                    + "dec_moviimporte) values(?,?,SYSDATE(),?,'003',?)";
            pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            pstm.setInt(2, cont);
            pstm.setString(3, codEmp);  // Usar el código de empleado recibido
            pstm.setDouble(4, importe);
            pstm.executeUpdate();

            // Confirmar transacción
            cn.commit();
        } catch (SQLException e) {
            try {
                if (cn != null) cn.rollback();
            } catch (SQLException el) {
                el.printStackTrace();
            }
            throw new RuntimeException(e.getMessage());
        } catch (Exception e) {
            try {
                if (cn != null) cn.rollback();
            } catch (SQLException el) {
                el.printStackTrace();
            }
            throw new RuntimeException("ERROR, en el proceso registrar depósito, intentelo más tarde.");
        } finally {
            try {
                if (cn != null) cn.close();
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }
    }
    
    // Método para registrar un retiro
    public void registrarRetiro(String cuenta, double importe, String codEmp) {
        Connection cn = null;
        try {
            cn = AccesoDB.getConnection();
            cn.setAutoCommit(false);

            // Paso 1: Leer datos de la cuenta
            String sql = "SELECT dec_cuensaldo, int_cuencontmov "
                       + "FROM cuenta "
                       + "WHERE chr_cuencodigo = ? AND vch_cuenestado = 'ACTIVO' "
                       + "FOR UPDATE";
            PreparedStatement pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            ResultSet rs = pstm.executeQuery();
            if (!rs.next()) {
                throw new SQLException("ERROR, cuenta no existe o no está activa.");
            }
            double saldo = rs.getDouble("dec_cuensaldo");
            int cont = rs.getInt("int_cuencontmov");
            rs.close();
            pstm.close();

            // Validar saldo suficiente
            if (saldo < importe) {
                throw new SQLException("ERROR, saldo insuficiente.");
            }

            // Paso 2: Actualizar la cuenta
            saldo -= importe;
            cont++;
            sql = "UPDATE cuenta "
            + "SET dec_cuensaldo = ?, int_cuencontmov = ? "
            + "WHERE chr_cuencodigo = ? AND vch_cuenestado = 'ACTIVO'";
            pstm = cn.prepareStatement(sql);
            pstm.setDouble(1, saldo);
            pstm.setInt(2, cont);
            pstm.setString(3, cuenta);
            pstm.executeUpdate();
            pstm.close();

            // Paso 3: Registrar movimiento
            sql = "insert into movimiento(chr_cuencodigo,"
                    + "int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,"
                    + "dec_moviimporte) values(?,?,SYSDATE(),?,'004',?)";
            pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            pstm.setInt(2, cont);
            pstm.setString(3, codEmp);
            pstm.setDouble(4, -importe);
            pstm.executeUpdate();
            cn.commit();
        } catch (SQLException e) {
            try { if (cn != null) cn.rollback(); } catch (SQLException ex) { ex.printStackTrace(); }
            throw new RuntimeException(e.getMessage());
        } finally {
            try { if (cn != null) cn.close(); } catch (SQLException e) { e.printStackTrace(); }
        }
    }

    // Método para registrar una transferencia
    public void registrarTransferencia(String cuentaOrigen, String cuentaDestino, double importe, String codEmp) {
        Connection cn = null;
        try {
            // Obtener la conexión
            cn = AccesoDB.getConnection();
            // Habilitar la transacción
            cn.setAutoCommit(false);
            // Retiro en cuenta origen
            registrarRetiro(cuentaOrigen, importe, codEmp);
            // Depósito en cuenta destino
            registrarDeposito(cuentaDestino, importe, codEmp);
            // Confirmar la transacción
            cn.commit();
        } catch (Exception e) {
            try {
                cn.rollback();
            } catch (Exception el) {
            }
            throw new RuntimeException("ERROR en el proceso de transferencia: " + e.getMessage());
        } finally {
            try {
                cn.close();
            } catch (Exception e) {
            }
        }
    }

}



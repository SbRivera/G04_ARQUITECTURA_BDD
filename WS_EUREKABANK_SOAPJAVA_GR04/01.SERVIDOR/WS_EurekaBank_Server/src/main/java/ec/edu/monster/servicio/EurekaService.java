/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.servicio;

import ec.edu.monster.db.AccesoDB;
import ec.edu.monster.modelo.Movimiento;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Base64;
import java.util.List;

/**
 *
 * @author JOIS
 */
public class EurekaService {

    //LOGIN
    private static final String USUARIO = "MONSTER";
    private static final String PASSWORD = generarHash("MONSTER9");

    public Boolean validarIngreso(String usuario, String password) {
        String hashIngresado = generarHash(password);
        return USUARIO.equals(usuario) && PASSWORD.equals(hashIngresado);
    }

    /**
     * Genera un hash SHA-256 de la contraseña proporcionada.
     */
    private static String generarHash(String password) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            byte[] hashBytes = digest.digest(password.getBytes(StandardCharsets.UTF_8));
            return Base64.getEncoder().encodeToString(hashBytes);
        } catch (NoSuchAlgorithmException e) {
            throw new RuntimeException("Error al generar el hash SHA-256", e);
        }
    }

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
                + "WHERE m.chr_cuencodigo = ? \n"
                + "ORDER BY m.dtt_movifecha DESC, m.int_movinumero DESC";

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

    public double registrarDeposito(String cuenta, double importe, String codEmp) {
        Connection cn = null;
        double saldo;
        try {
            cn = AccesoDB.getConnection();
            cn.setAutoCommit(false);

            String sql = "select dec_cuensaldo, int_cuencontmov "
                    + "from cuenta "
                    + "where chr_cuencodigo = ? and vch_cuenestado = 'ACTIVO' "
                    + "for update";
            PreparedStatement pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            ResultSet rs = pstm.executeQuery();
            if (!rs.next()) {
                throw new SQLException("ERROR, cuenta no existe, o no esta activa");
            }
            saldo = rs.getDouble("dec_cuensaldo");
            int cont = rs.getInt("int_cuencontmov");
            rs.close();
            pstm.close();

            // paso 2: actualizar
            saldo += importe;
            cont++;
            sql = "update cuenta "
                    + "set dec_cuensaldo = ?, "
                    + "int_cuencontmov = ? "
                    + "where chr_cuencodigo = ? and vch_cuenestado = 'ACTIVO'";
            pstm = cn.prepareStatement(sql);
            pstm.setDouble(1, saldo);
            pstm.setInt(2, cont);
            pstm.setString(3, cuenta);
            pstm.executeUpdate();
            pstm.close();

            // paso 3: movimiento
            sql = "insert into movimiento(chr_cuencodigo,"
                    + "int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,"
                    + "dec_moviimporte) values(?,?,SYSDATE(),?,'003',?)";
            pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            pstm.setInt(2, cont);
            pstm.setString(3, codEmp);
            pstm.setDouble(4, importe);
            pstm.executeUpdate();

            cn.commit();
        } catch (SQLException e) {
            try {
                if (cn != null) {
                    cn.rollback();
                }
            } catch (Exception el) {
            }
            throw new RuntimeException(e.getMessage());
        } catch (Exception e) {
            try {
                if (cn != null) {
                    cn.rollback();
                }
            } catch (Exception el) {
            }
            throw new RuntimeException("ERROR, en el proceso registrar deposito, intentelo mas tarde.");
        } finally {
            try {
                if (cn != null) {
                    cn.close();
                }
            } catch (Exception e) {
            }
        }
        // devolvemos el saldo actualizado
        return saldo;
    }

    public double registrarRetiro(String cuenta, double importe, String codEmp) {
        Connection cn = null;
        double saldo;
        try {
            cn = AccesoDB.getConnection();
            cn.setAutoCommit(false);

            String sql = "select dec_cuensaldo, int_cuencontmov "
                    + "from cuenta "
                    + "where chr_cuencodigo = ? and vch_cuenestado = 'ACTIVO' "
                    + "for update";
            PreparedStatement pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            ResultSet rs = pstm.executeQuery();

            if (!rs.next()) {
                throw new SQLException("ERROR: La cuenta no existe o no está activa.");
            }

            saldo = rs.getDouble("dec_cuensaldo");
            int cont = rs.getInt("int_cuencontmov");
            rs.close();
            pstm.close();

            if (saldo < importe) {
                throw new SQLException("ERROR: Saldo insuficiente.");
            }

            saldo -= importe;
            cont++;
            sql = "update cuenta set dec_cuensaldo = ?, int_cuencontmov = ? "
                    + "where chr_cuencodigo = ? and vch_cuenestado = 'ACTIVO'";
            pstm = cn.prepareStatement(sql);
            pstm.setDouble(1, saldo);
            pstm.setInt(2, cont);
            pstm.setString(3, cuenta);
            pstm.executeUpdate();
            pstm.close();

            sql = "insert into movimiento(chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte) "
                    + "values (?, ?, SYSDATE(), ?, '004', ?)";
            pstm = cn.prepareStatement(sql);
            pstm.setString(1, cuenta);
            pstm.setInt(2, cont);
            pstm.setString(3, codEmp);
            pstm.setDouble(4, importe);  // si quieres, aquí podrías guardar negativo
            pstm.executeUpdate();
            cn.commit();

            System.out.println("Retiro registrado correctamente.");
        } catch (SQLException e) {
            try {
                if (cn != null) {
                    cn.rollback();
                }
            } catch (SQLException ex) {
            }
            System.err.println("Error en SQL: " + e.getMessage());
            throw new RuntimeException(e.getMessage());
        } finally {
            try {
                if (cn != null) {
                    cn.close();
                }
            } catch (SQLException e) {
            }
        }
        return saldo;
    }

    public double registrarTransferencia(String cuentaOrigen, String cuentaDestino, double importe, String codEmp) {
        Connection cn = null;
        double saldoOrigen;
        try {
            cn = AccesoDB.getConnection();
            cn.setAutoCommit(false);

            // Retiro en cuenta origen (te devolverá el nuevo saldo)
            saldoOrigen = registrarRetiro(cuentaOrigen, importe, codEmp);

            // Depósito en cuenta destino (no necesitamos su saldo aquí)
            registrarDeposito(cuentaDestino, importe, codEmp);

            cn.commit();
        } catch (Exception e) {
            try {
                if (cn != null) {
                    cn.rollback();
                }
            } catch (Exception el) {
            }
            throw new RuntimeException("ERROR en el proceso de transferencia: " + e.getMessage());
        } finally {
            try {
                if (cn != null) {
                    cn.close();
                }
            } catch (Exception e) {
            }
        }
        return saldoOrigen;
    }

}

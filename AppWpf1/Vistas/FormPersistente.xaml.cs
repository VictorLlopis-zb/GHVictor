using AppWpf1.Datos;
using AppWpf1.DTO;
using AppWpf1.Interfaces;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using AppWpf1.Vistas;
using Microsoft.VisualBasic;
using Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AppWpf1.Vistas
{
    public partial class FormPersistente : Window
    {
        // Campo de clase
        // Constantes estáticas para visibilidad
        public static readonly Visibility BtnOn = Visibility.Visible;
        public static readonly Visibility BtnOff = Visibility.Hidden;

        private ObservableCollection<PersonaIdentidadFormulario> listaFormulario;
        private dynamic? crud;
        private string boton = null;
        private string nombreElemento = string.Empty;
        private string cedulaElemento = string.Empty;


        public FormPersistente(Type tipoPersistente)
        {
            InitializeComponent();
            
            try
            {
                object? lista = null;

                // 1. Crear instancia para verificar contrato IPersistente<T>
                var instancia = Activator.CreateInstance(tipoPersistente);
                bool NoPIF = !((dynamic)instancia).EsPIF();

                // Caso especial: PersonaIdentidadFormulario con EsPIF == true

                //if (instancia is IPersistente<PersonaIdentidadFormulario> persistente && persistente.EsPIF())
                if (!NoPIF)   //caso de PersonaIdentidadFormulario generar listaintermedia
                {
                    // Usar conversor para regenerar lista intermedia desde la persistente real
                    lista = ConversorPersonaIdentidad.GenerarListaFormulario(PersonaIdentidad.ListaPersistente);
                }
                else
                {

                    // Caso genérico: obtener lista persistente desde BaseLocal
                    var metodo = typeof(BaseLocal)
                        .GetMethod("ObtenerLista")
                        .MakeGenericMethod(tipoPersistente);

                    lista = metodo?.Invoke(null, null);
                }

                // 2. Asignar al DataGrid
                dgvRegistros.ItemsSource = (System.Collections.IEnumerable)lista!;

                // 3. Generar formulario dinámico
                var tipoGenerador = typeof(FormularioGeneral<>).MakeGenericType(tipoPersistente);
                dynamic generador = Activator.CreateInstance(tipoGenerador);
                var formulario = generador.GenerarFormulario();

                // Vincular datos si corresponde
                var enumerador = ((System.Collections.IEnumerable)lista!).GetEnumerator();
                if (enumerador.MoveNext())
                {
                    formulario.DataContext = enumerador.Current;
                }

                // 4. Crear instancia CRUD
                var tipoCrud = typeof(FormCRUD<>).MakeGenericType(tipoPersistente);
                var entidad = Activator.CreateInstance(tipoPersistente);

                crud = Activator.CreateInstance(
                    tipoCrud,
                    entidad,
                    dgvRegistros,
                    lista
                );

                // 5. Incrustar formulario en el contenedor
                ventanaCrud.Content = crud.Formulario;
                inicio();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear FormPersistente: {ex.Message}", "Diagnóstico");
            }
        }
        private void ManejarBotones(Visibility btn0, Visibility btn1, Visibility btn2, Visibility btn3, Visibility btn4, bool selecc = true)
        {
            btnCancelar.Visibility = btn0;
            btnCrear.Visibility = btn1;
            btnModificar.Visibility = btn2;
            btnEliminar.Visibility = btn3;
            btnAceptar.Visibility = btn4;
            //dgvRegistros.SelectionMode = DataGridSelectionMode.Single;
            //dgvRegistros.UnselectAll(); // limpia cualquier selección            
            if (selecc)
            
                // Deshabilitar selección pero mantener el listado visible
                
                dgvRegistros.IsHitTestVisible = false; // bloquea clics en filas
            
                       
        }
        private void MostrarInstrucciones(string accion)
        {
            switch (accion)
            {
                case "inicio":
                    this.Title = "Formulario CRUD Selección Inicial";
                    txtInstrucciones.Text = "Para Nuevo Seleccione Crear.\n Para Modificar o Eliminar Seleccione Elemento";
                    break;
                case "crear":
                    this.Title = "Crear Nuevo Elemento";
                    txtInstrucciones.Text = "Ingrese los datos del nuevo registro.\n Cancelar para suspender.\n Aceptar para guardar.";
                    break;

                case "modificar":
                    this.Title = "Modificar Datos de " + nombreElemento;
                    txtInstrucciones.Text = "Seleccione un registro de la lista, edite los campos y presione Aceptar.";
                    break;

                case "eliminar":
                    this.Title = "Eliminar a " + nombreElemento";
                    txtInstrucciones.Text = "Seleccione un registro de la lista y confirme la eliminación.";
                    break;

                case "aceptar":
                    txtInstrucciones.Text = "Confirme la operación actual. Los cambios se aplicarán al registro.";
                    break;

                case "Salir":
                    txtInstrucciones.Text = "Cierre la ventana para finalizar el flujo CRUD.";
                    break;

                default:
                    txtInstrucciones.Text = "?accion Esta opcion no existe.";
                    break;
            }
        }
        private void dgvRegistros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seleccionado = dgvRegistros.SelectedItem;
            if (seleccionado != null)
            {
                // Aquí actualizas tu formulario incrustado
                boton = null;   //ojo con crear en noPIF que pasa por aqui
                ManejarBotones(BtnOn, BtnOff, BtnOn, BtnOn, BtnOff);
                var cedul  = dgvRegistros.Columns[0].GetCellContent(seleccionado) as TextBlock;
                cedulaElemento = cedul?.Text;

                var persona = PersonaIdentidad.ListaPersistente
                                              .FirstOrDefault(p => p.Cedula == cedulaElemento);
                nombreElemento = persona != null ? persona.NombrePlano() : string.Empty;

                //crud.Formulario.DataContext = seleccionado;
            }
        }
        private bool VerSiNuevo(string bot)
        {
            if (boton != null && boton != bot)
            {
                if (!HelperConfirmar.Preguntar("¿Cancelar Operación en curso?"))
                    return false;   // no se cancela
            }
            return true;// sigue nueva operación
        }
        private bool ValidarSeleccion(string boto)
        {
            if (!VerSiNuevo(boto)) return false;

            if (dgvRegistros.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un elemento antes de continuar.");
                return false;
            }
            boton = boto;
            return true;
        }
        private void inicio(bool selecc = false)
        {
            ManejarBotones(BtnOn, BtnOn, BtnOff, BtnOff, BtnOff, false);
            MostrarInstrucciones("inicio");
            boton = null;
            dgvRegistros.SelectedItem = null;
            if (selecc) crud.Cancelar();
        }
        

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            inicio(false);
        }
        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            ManejarBotones(BtnOn, BtnOff, BtnOff, BtnOff, BtnOn);
            boton = "crear";
            MostrarInstrucciones(boton);
            crud.Crear();
            //crud.GetType().GetMethod("Crear")?.Invoke(crud, null);
        }
        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(nombreElemento);

            ManejarBotones(BtnOn, BtnOff, BtnOn, BtnOff, BtnOn);
            boton = "modificar";
            crud.GetType().GetMethod("Modificar")?.Invoke(crud, null);
        }
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            ManejarBotones(BtnOn, BtnOff, BtnOff, BtnOn, BtnOn);
            boton = "eliminar";
            crud.GetType().GetMethod("Eliminar")?.Invoke(crud, null);
        }


        private void btnAceptar_Click(object sender, RoutedEventArgs e)
            => crud.GetType().GetMethod("Aceptar")?.Invoke(crud, null);

        private void btnSalir_Click(object sender, RoutedEventArgs e)
            => Close();
        private void dgvRegistros_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var propertyType = (e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor)?.PropertyType;

            // Ocultar Password
            if (e.PropertyName == "Password")
            {
                e.Cancel = true;
                return;
            }

            // Formatear fechas
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                if (e.Column is DataGridTextColumn col && col.Binding is Binding binding)
                {
                    binding.StringFormat = "dd/MM/yyyy";
                }
            }
        }

    }

}

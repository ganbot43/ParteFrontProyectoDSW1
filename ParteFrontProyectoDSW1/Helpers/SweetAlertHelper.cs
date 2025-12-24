namespace ParteFrontProyectoDSW1.Helpers
{
    public class SweetAlertHelper
    {
        public static string SweetAlert(string title, string text, string icon = "info")
        {
            return $@"
                <script>
                    Swal.fire({{
                        title: '{title}',
                        text: '{text}',
                        icon: '{icon}'
                    }});
                </script>";
        }

        public static string SweetToast(string title, string icon = "info", int timer = 5000)
        {
            return $@"
                <script>
                    const Toast = Swal.mixin({{
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: {timer},
                        timerProgressBar: true,
                        didOpen: (toast) => {{
                            toast.onmouseenter = Swal.stopTimer;
                            toast.onmouseleave = Swal.resumeTimer;
                        }}
                    }});
                    Toast.fire({{
                        icon: '{icon}',
                        title: '{title}'
                    }});
                </script>";
        }
    }
}

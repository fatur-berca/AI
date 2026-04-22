using System.Web;
using System.Web.Optimization;

namespace hms_tom_dev
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/init-load").Include(
                     "~/assets/js/Highcharts.js",
                     "~/assets/js/jquery-1.12.4.min.js",
                     "~/assets/js/jquery-ui.min.js",
                     "~/assets/js/main.min.js",
                     "~/assets/js/jquery.dataTables.min.js",
                     "~/assets/js/dataTables.fixedColumns.min.js",
                     "~/assets/js/moment.js",
                     "~/assets/js/bootstrap-datetimepicker.min.js",
                     "~/assets/js/image-editor/jquery-mousewheel.js",
                     "~/assets/js/image-editor/hammer.min.js",
                     "~/assets/js/image-editor/jquery-hammer.js",
                     "~/assets/js/image-editor/imgViewer.js",
                     "~/assets/js/image-editor/imgNotes.js",
                     //"~/assets/js/lightbox.min.js",
                     "~/assets/js/jquery.matchHeight-min.js",
                     
                     //"~/assets/js/select2.min.js",
                     "~/assets/js/select2-3.5.1.min.js",
                     "~/assets/js/jquery.dataTables.yadcf.min.js",
                     "~/assets/js/jquery.tree.min.js",
                     "~/assets/js/dataTables.rowsGroup.js",
                     "~/Scripts/jquery.easy-autocomplete.min.js",
                     "~/assets/js/bootstrap-select.min.js",
                     "~/assets/js/scripts.js",
                     "~/assets/js/fieldChooser.min.js",
                     "~/assets/js/jquery.treetable.js",
                     "~/assets/js/hummingbird-treeview-1.4.js",
                     "~/assets/js/jquery.pivot.js",
                     "~/assets/js/adapter.js",
                     "~/assets/js/lib.js",
                     "~/Scripts/bootbox.min.js",
                     "~/assets/js/jquery-filestyle.min.js",
                     "~/assets/js/validator.js",
                     "~/assets/js/planit.min.js",
                     "~/assets/js/canvas2image.js",
                     "~/assets/js/html2canvas.js",
                     "~/Scripts/fabric.min.js",
                     "~/assets/js/global.js",
                     "~/assets/js/spectrum.js",
                     "~/assets/js/jquery-sortable-min.js",
                     "~/assets/js/lodash.js"
                    //"~/assets/js/fieldChooser.min.js"
            ));
        }
    }
}

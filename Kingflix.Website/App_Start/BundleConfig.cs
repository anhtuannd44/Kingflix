using System.Web.Optimization;

namespace Kingflix.Website
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/template-js").Include(
                      "~/Content/theme/vendors/js/vendor.bundle.base.js",
                      "~/Content/theme/js/misc.js",
                      "~/Content/plugin/upload/js/jquery.dm-uploader.min.js",
                      "~/Scripts/moment-with-locales.min.js",
                      "~/Content/plugin/datetimepicker/js/bootstrap-datetimepicker.min.js",
                      "~/Content/plugin/toastr/toastr.min.js",
                      "~/Content/plugin/summernote/summernote-bs4.js",
                      "~/Content/theme/js/summernote.js",
                      "~/Content/plugin/select2/dist/js/select2.full.min.js",
                      "~/Content/plugin/select2/dist/js/i18n/vi.js",
                      "~/Scripts/upload/image-library.js",
                      "~/Content/plugin/chart/dist/Chart.min.js",
                      "~/Content/plugin/nprogress/nprogress.js",
                      "~/Content/plugin/bootstrap-table/dist/bootstrap-table.min.js",
                      "~/Content/plugin/bootstrap-table/dist/locale/bootstrap-table-vi-VN.min.js",
                      "~/Scripts/clipboard/dist/clipboard.min.js",
                      "~/Scripts/jquery.lazyload.js",
                      "~/Content/plugin/tiny-slider/dist/tiny-slider.js"));

            bundles.Add(new StyleBundle("~/Content/template-css").Include(
                      "~/Content/theme/vendors/mdi/css/materialdesignicons.min.css",
                      "~/Content/theme/vendors/css/vendor.bundle.base.css",
                      "~/Content/plugin/datetimepicker/css/bootstrap-datetimepicker.css",
                      "~/Content/plugin/upload/css/jquery.dm-uploader.min.css",
                      "~/Content/font.css",
                      "~/Content/PagedList.css",
                      "~/Content/plugin/chart/dist/Chart.min.css",
                      "~/Content/plugin/toastr/toastr.css",
                      "~/Content/plugin/summernote/summernote-bs4.css",
                      "~/Content/theme/css/style.css",
                      "~/Content/plugin/select2/dist/css/select2.min.css",
                      "~/Content/plugin/bootstrap-table/dist/bootstrap-table.min.css",
                      "~/Content/theme-custom.css",
                      "~/Content/plugin/nprogress/nprogress.css",
                      "~/Content/plugin/tiny-slider/dist/tiny-slider.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/style-css").Include(
                      "~/Content/home.css"));

            bundles.Add(new ScriptBundle("~/bundles/home-js").Include(
                      "~/Scripts/rating/rater.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/order-js").Include(
                      "~/Scripts/front/order.js"));
        }
    }
}
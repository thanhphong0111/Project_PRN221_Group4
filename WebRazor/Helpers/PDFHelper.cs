using WebRazor.Models;
using SelectPdf;

namespace WebRazor.Helpers
{
    public class PDFHelper
    {
        public static PdfDocument GenPDFInvoice(string name, string address, Dictionary<Product, OrderDetail> listProducts)
        {
            var html = $@"<h1  style='padding-left:300px;color: #ff5733 '>Customer Infors</h1><br>
                        <span style='padding-left:100px;color:blue'>Customer name: {name} </span> <br>
                        <span style='padding-left:100px;color:blue'>Address: {address}  </span><br>";
            decimal totalPrice = 0;
            foreach (var product in listProducts)
            {
                html += $@"<span style='padding-left:100px;color:blue'>Product name: {product.Key.ProductName} </span><br>
                           <span style='padding-left:100px;color:blue'>Quantity: {product.Value.Quantity}</span><br> 
                           <span style='padding-left:100px;color:blue'>Unit price: {product.Value.UnitPrice}$ </span><br>
                           <span style='padding-left:100px;color:blue'>Price: {product.Value.UnitPrice * product.Value.Quantity} $</span><br><br>";
                totalPrice += product.Value.UnitPrice * product.Value.Quantity;
            }
            html += $@"<h2 style='padding-left:100px;color: #ff5733'>Total price: {totalPrice}$</h2>";

            HtmlToPdf converter = new HtmlToPdf();

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(html);
            return doc;
        }
    }
}

import logging
import urllib.request
import azure.functions as func
from pdf2image import convert_from_path

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    url = req.params.get('url')
    if not url:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            url = req_body.get('url')

    urllib.request.urlretrieve(url, 'request.pdf')

    pages = convert_from_path('request.pdf', 500)

    i = 1
    for page in pages:
        page.save('out' + i + '.jpg', 'JPEG')
        i = i + 1 

    if name:
        return func.HttpResponse(f"Hello {name}!")
    else:
        return func.HttpResponse(
             "Please pass a name on the query string or in the request body",
             status_code=400
        )


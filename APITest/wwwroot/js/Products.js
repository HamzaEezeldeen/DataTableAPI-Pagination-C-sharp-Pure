
async function getCategories(selectEml) {
    let url = "https://localhost:7215/api/Category";
    fetch(url, {
        method: "GET"
    })
        .then((response) => response.json())
        .then((data) => {
            //console.log(data);
            RenderOption(selectEml, data);
            RenderOption(document.querySelector('#CategoryId'), data);

        })
        .catch((error) => {
            console.error(error);
        });

}

async function getProducts(tbl_container, categoryid) {

    tbl_container.innerHTML = '';

    let url = "https://localhost:7215/api/Product/all";
    if (categoryid !== null)
        url = `https://localhost:7215/products_in/${categoryid}`;

    fetch(url, {
        method: "GET"
    })
        .then((response) =>  response.json())
        .then((data) => {
            //console.log(data);
            data.forEach(p => RanderRow(tbl_container,p));
        })
        .catch((er) => {
            console.error(er);
        });

}

function RanderRow(tbl_container,{ categoryId, description, id, isAvailable, name, price, sku }) {

    let row = document.createElement('tr');
    row.className = '';
    row.innerHTML =
        `  
        <td>${id}</td>
        <td>${sku}</td>
        <td>${name}</td>
        <td>${price}</td>
        <td><input type="checkbox" name="IsAvailable" ${(isAvailable?'checked':'')} /></td>
        `;
    tbl_container.appendChild(row);
}

function RenderOption(selectElement, lstCategories) {
    //alert(lstCategories + ' ' + lstCategories.length)
    for (element of lstCategories) {
        // console.log(element);
        let li = document.createElement("option");
        li.value = element.id;
        li.innerHTML = element.name;
        selectElement.appendChild(li);
    }
}


async function PostProductByForm(formData) {

    let url = "https://localhost:7215/api/Product";

    fetch(url, {
        method: "POST",
        body: formData
    })
        .then((resp) => {

            if ([200, 201].indexOf(resp.status) !== -1) {

                resp.json().then((product) => {

                    document.querySelector('#btnClose').click();

                    document.querySelector('#lstCategory').value = product.categoryId;

                    getProducts(table, product.categoryId);

                    document.querySelector('#TxtPageMessage').innerHTML = `A new product with id ${product["id"]} has been added.`;

                    formAdd.reset();
                }
                );

            }
            else {

                resp.json().then(error => {

                    document.querySelector('#txtModelMessage').innerHTML = `Error: ${error.title}`;

                });
            }

        })
        .catch((error) => {
            console.error(error);
        })


}



let formAdd = document.querySelector("#frmAdd");
let selectElement = document.querySelector('#lstCategory');
let table = document.querySelector('#tblProducts tbody');
getProducts(table,null);
getCategories(selectElement)

selectElement.addEventListener("change", (event) => {

    if (event.target.value == '-1')
        getProducts(table,null);
    else
        getProducts(table,event.target.value);
});

formAdd.addEventListener('submit', (event) => {

    event.preventDefault();


    document.querySelector('#TxtPageMessage').innerHTML = '';
    document.querySelector('#txtModelMessage').innerHTML = '';

    let formData = new FormData();
    formData.append("Id", 0);
    formData.append("Sku", formAdd.Sku.value);
    formData.append("Name", formAdd.Name.value);
    formData.append("Description", formAdd.Description.value);
    formData.append("Price", parseFloat(formAdd.Price.value));
    formData.append("IsAvailable", formAdd.IsAvailable.checked);
    formData.append("CategoryId", formAdd.CategoryId.value);
    //formData.append("Category",null);

    PostProductByForm(formData);


});

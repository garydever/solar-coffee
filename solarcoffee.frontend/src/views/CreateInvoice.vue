<template>
    <div>
        <h1 id="invoiceTitle">
            Create Invoice
        </h1>
        <hr />

        <!--consider refactoring to put these steps into 3 seperate components:-->
        <div class="invoice-step" v-if="invoiceStep === 1">
            <h2>Step 1: Select Customer</h2>
            <div v-if="customers.length" class="invoice-step-detail">
                <label for="customer">Customer:</label>
                <select v-model="selectedCustomerId"
                        class="invoice-customers"
                        id="customer">
                    <option disabled value="">Please select a Customer</option>
                    <option v-for="c in customers" :value="c.id" :key="c.id">
                        {{
            c.firstName + " " + c.lastName
                        }}
                    </option>
                </select>
            </div>
        </div>

        <div class="invoice-step" v-if="invoiceStep === 2">
            <h2>Step 2: Create Order</h2>
            <div v-if="inventory.length" class="invoice-step-detail">
                <label for="product">Product:</label>
                <select v-model="newItem.product" class="invoiceLineItem" id="product">
                    <option disabled value="">Please select a product</option>
                    <option v-for="i in inventory" :value="i.product" :key="i.product.id">
                        {{i.product.name}}
                    </option>
                </select>
                <label for="quantity">Quantity:</label>
                <input v-model="newItem.quantity" id="quantity" type="number" min="0" />
            </div>

            <div class="invoice-step-actions">
                <solar-button :diabled="!newItem.product || !newItem.quantity" @button:click="addLineItem">
                    Add Line Item
                </solar-button>
                <solar-button :diabled="!lineItems.length" @button:click="finalizeOrder">
                    Finalize Order
                </solar-button>
            </div>

            <div class="invoice-order-list" v-if="lineItems.length">
                <div class="runningTotal">
                    <h3>Running Total:</h3>
                    {{runningTotal | price}}
                </div>
                <hr />
                <table class="table">
                    <thead>
                        <tr>
                            <th>Product</th>
                            <th>Description</th>
                            <th>Qty.</th>
                            <th>Price</th>
                            <th>Subtotal</th>
                        </tr>
                    </thead>
                    <tr v-for="lineItem in lineItems" :key="`index_${lineItem.product.id}`">
                        <td class="item-col">{{ lineItem.product.name }}</td>
                        <td class="item-col">{{ lineItem.product.description }}</td>
                        <td class="item-col">{{ lineItem.quantity }}</td>
                        <td class="item-col">{{ lineItem.product.price | price }}</td>
                        <td class="item-col">{{ lineItem.product.price * lineItem.quantity | price }}</td>
                    </tr>
                </table>
            </div>

        </div>

        <div class="invoice-step" v-if="invoiceStep === 3">
            <h2>Step 3: Review and Submit</h2>
            <solar-button @button:click="submitInvoice">Submit Invoice</solar-button>
            <hr />
            <div class="invoice-step-detail" id="invoice" ref="invoice" >
                <div class="invoice-logo">
                    <img id="imgLogo" alt="Solar Coffee Logo" src="../assets/images/solar_coffee_logo.png" />
                    <h3>123 Fake St.</h3>
                    <h3>Coffee Town, NJ</h3>
                    <h3>USA</h3>

                    <div class="invoice-order-list" v-if="lineItems.length">
                        <div class="invoice-header">
                            <h3>Invoice: {{ new Date() | humanizeDate }}</h3>
                            <h3>Customer: {{ selectedCustomer.firstName + ' ' + selectedCustomer.lastName }}  </h3>
                            <h3>
                                Address: {{ selectedCustomer.primaryAddress.addressLine1 }}
                            </h3>
                            <h3 v-if="selectedCustomer.primaryAddress.addressLine2">
                                {{ selectedCustomer.primaryAddress.addressLine2 }}
                            </h3>
                            <h3>
                                {{ selectedCustomer.primaryAddress.city }},
                                {{ selectedCustomer.primaryAddress.state }},
                                {{ selectedCustomer.primaryAddress.postalCode }}
                            </h3>
                            <h3>
                                {{ selectedCustomer.primaryAddress.country }}
                            </h3>
                        </div>
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Description</th>
                                    <th>Qty.</th>
                                    <th>Price</th>
                                    <th>Subtotal</th>
                                </tr>
                            </thead>
                            <tr v-for="lineItem in lineItems"
                                :key="`index_${lineItem.product.id}`">
                                <td>{{ lineItem.product.name }}</td>
                                <td>{{ lineItem.product.description }}</td>
                                <td>{{ lineItem.quantity }}</td>
                                <td>{{ lineItem.product.price }}</td>
                                <td>
                                    {{ (lineItem.product.price * lineItem.quantity) | price }}
                                </td>
                            </tr>
                            <tr>
                                <th colspan="4"></th>
                                <th>Grand Total</th>
                            </tr>
                            <tfoot>
                                <tr>
                                    <td colspan="4" class="due">Balance due upon receipt:</td>
                                    <td class="price-final">{{ runningTotal | price }}</td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <hr />

        <div class="invoice-step-actions">
            <solar-button @button:click="prev" :disabled="!canGoPrev">Previous</solar-button>
            <solar-button @button:click="next" :disabled="!canGoNext">Next</solar-button>
            <solar-button @button:click="startOver">Start Over</solar-button>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { IInvoice, ILineItem } from '@/types/IInvoice'
    import { ICustomer } from '@/types/Customer'
    import { IProductInventory } from '@/types/Product'
    import CustomerService from '../services/customer-service';
    import { InventoryService } from '../services/inventory-service';
    import InvoiceService from '../services/invoice-service';
    import SolarButton from '@/components/SolarButton.vue';
    import jsPDF from 'jspdf';
    import html2canvas from 'html2canvas';

    const customerService = new CustomerService();
    const inventoryService = new InventoryService();
    const invoiceService = new InvoiceService();


    @Component({ name: 'CreateInvoice', components: { SolarButton } })

    export default class CreateInvoice extends Vue {
        invoiceStep = 1;
        invoice: IInvoice = { customerId: 0, lineItems: [], createdOn: new Date(), updatedOn: new Date() };
        customers: ICustomer[] = [];
        selectedCustomerId = 0;
        inventory: IProductInventory[] = [];
        lineItems: ILineItem[] = [];
        newItem: ILineItem = { product: undefined, quantity: 0 }

        //computed property with "get" syntax:
        get runningTotal() {
            return this.lineItems.reduce((a, b) => a + (b['product']['price'] * b['quantity']), 0);
        }

        get canGoNext() {
            if (this.invoiceStep === 1) {
                return this.selectedCustomerId !== 0;
            }
            if (this.invoiceStep === 2) {
                return this.lineItems.length;
            }
            if (this.invoiceStep === 3) {
                return false;
            }
            return false;
        }
        //why didn't we use get syntax to make this a computed property as well?
        get canGoPrev() {
            return this.invoiceStep !== 1;
        }

        get selectedCustomer() {
            return this.customers.find(c => c.id == this.selectedCustomerId);
        }

        async submitInvoice(): Promise<void> {
            this.invoice = {
                customerId: this.selectedCustomerId,
                lineItems: this.lineItems
            }

            await invoiceService.makeNewInvoice(this.invoice);
            this.downloadPdf();
            await this.$router.push("/orders");
        }

        downloadPdf() {
            const pdf = new jsPDF("p", "pt", "a4", true);
            const invoice = document.getElementById('invoice');
            const width = this.$refs.invoice?.clientWidth;
            const height = this.$refs.invoice?.clientHeight;


            if (invoice) {
                html2canvas(invoice).then(canvas => {
                    const image = canvas.toDataURL('image/png');
                    pdf.addImage(image, 'PNG', 0, 0, width * 0.65, height * 0.65);
                    pdf.save('invoice');
                })
            }
            
        }

        addLineItem() {
            let newItem: ILineItem = {
                product: this.newItem.product,
                quantity: Number(this.newItem.quantity)
            };

            let existingItems = this.lineItems.map(item => item.product.id);

            if (existingItems.includes(newItem.product.id)) {
                let lineItem = this.lineItems.find(item => item.product.id === newItem.product.id);

                //let currentQuantity = Number(lineitem.quantity);
                //let updatedQuantity = currentQuantity += newItem.quantity;
                //lineItem.quantity = updatedQuantity;
                lineItem.quantity = Number(lineItem.quantity) + newItem.quantity;
            } else {
                this.lineItems.push(this.newItem);
            }

            this.newItem = { product: undefined, quantity: 0 };
        }

        startOver(): void {
            this.invoice = { customerId: 0, lineItems: [], createdOn: new Date, updatedOn: new Date };
            this.lineItems = [];
            this.selectedCustomerId = 0;
            this.invoiceStep = 1;
        }

        finalizeOrder() {
            this.invoiceStep = 3;
        }

        prev(): void {
            if (this.invoiceStep === 1) {
                return;
            }
            this.invoiceStep -= 1;
        }

        next(): void {
            if (this.invoiceStep === 3) {
                return;
            }
            this.invoiceStep += 1;
        }

        async initialize(): Promise<void> {
            //1. a way to get customers using "then" syntax
            customerService.getCustomers().then(res => {
                this.customers = res;
            }).catch((e) => {
                //This is where we could create an error modal for displaying error message.
                console.log('Error getting customers!');
                console.log('Our error: '+e);
            })

            //2. or we could just do it like they did in the tutorial:
            // this.customers = await customerService.getCustomers();

            this.inventory = await inventoryService.getInventory();
        }
        

        async created() {
            await this.initialize();
        }
    }
</script>

<style scoped lang="scss">
    @import "@/scss/global.scss";

    .invoice-step-actions {
        display: flex;
        width: 100%;
    }

    .invoice-step {
    }

    .invoice-step-detail {
        margin: 1.2rem;
    }
    .invoice-order-list {
        margin-top: 1.2rem;
        padding: 0.8rem;
        .line-item {
                       display: flex;
                       border-bottom: 1px dashed #ccc;
                       padding: 0.8rem;
                       .item-col {
                                     flex-grow: 0.8rem;
                                 }
                   }
    }
    .invoice-customers {
        display: block;
    }
    .invoice-item-actions {
        display: flex;
    }

    .price-pre-tax {
        font-weight: bold;
    }

    .price-final {
        font-weight: bold;
        color: $solar-green;
    }
    .due {
        font-weight: bold;
    }

    .invoice-header {
        width: 100%;
        margin-bottom: 1.2rem;
    }

    .invoice-logo {
        padding-top: 1.4rem;
        text-align: center;
        img

    {
        width: 280px;
    }

    }

</style>
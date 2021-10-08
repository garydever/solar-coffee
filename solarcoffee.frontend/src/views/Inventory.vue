<template>
   <div class="inventory-container">
       <h1 id="inventoryTitle">
           Inventory Dashboard 
       </h1>
       <hr />

       <div class="inventory-actions">
           <solar-button @click.native="showNewProductModal" id="addNewBtn">
               Add New Item
           </solar-button>
           <solar-button @click.native="showShipmentModal" id="recieveShipmentBtn">
               Receive Shipment
           </solar-button>
       </div>

       <table id="inventoryTable" class="table">
           <tr>
               <th>Item</th>
               <th>Quantity On-hand</th>
               <th>Unit Price</th>
               <th>Taxable</th>
               <th>Delete</th>
           </tr>
           <tr v-for="item in inventory" :key="item.id">
               <td>
                   {{ item.product.name }}
               </td>
               <td>
                   {{ item.quantityOnHand }}
               </td>
               <td>
                   {{ item.product.price | price }}
               </td>
               <td>
                   <span v-if="item.product.isTaxable">
                       Yes
                   </span>
                   <span v-else>
                       No
                   </span>
               </td>
               <td>
                   <div>
                       X
                   </div>
               </td>
           </tr>
       </table>
       <new-product-modal 
                          v-if="isNewProductVisible"
                          @close="closeModals" 
                          @save:product="saveNewProduct" />
       <shipment-modal 
                       v-if="isShipmentVisible" 
                       :inventory="inventory" 
                       @close="closeModals" 
                       @save:shipment="saveNewShipment"/>
   </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { IProductInventory, IProduct } from "@/types/Product";
    import SolarButton from '@/components/SolarButton.vue';
    import NewProductModal from '@/components/modals/NewProductModal.vue';
    import ShipmentModal from "@/components/modals/ShipmentModal.vue";
import { IShipment } from '../types/Shipment';

    @Component({
        name: 'Inventory',
        components: { SolarButton, NewProductModal, ShipmentModal }
    })

    export default class Inventory extends Vue {
        //data
        isNewProductVisible = false;
        isShipmentVisible = false;
        inventory: IProductInventory[] = [
            {
                id: 1,
                product: { id: 1, name: 'Some Product', description: 'Good stuff', price: 100, createdOn: new Date(), updatedOn: new Date(), isTaxable: true, isArchived: false },
                quantityOnHand: 100,
                idealQuantity: 100
               
            },
            {
                id: 2,
                product: { id: 2, name: 'Some Other Product', description: 'More Good stuff', price: 200, createdOn: new Date(), updatedOn: new Date(), isTaxable: false, isArchived: false },
                quantityOnHand: 50,
                idealQuantity: 40

            },
        ];
        //methods
        closeModals() {
            this.isNewProductVisible = false;
            this.isShipmentVisible = false;
        }
        showNewProductModal() {
            this.isNewProductVisible = true;
        };
        showShipmentModal() {
            this.isShipmentVisible = true;
        };
        saveNewProduct(newProduct: IProduct) {
            console.log("saveNewProduct:");
            console.log(newProduct)
        }
        saveNewShipment(shipment: IShipment) {
            console.log("saveNewShipment:");
            console.log(shipment);
        }

    }
</script>

<style scoped lang="scss">
    @import '@/scss/global.scss';
</style>
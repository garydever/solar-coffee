<template>
    <div>
        <h1 id="customersTitle">
            Manage Customers
        </h1>
        <hr />
        <div class="customer-actions">
            <solar-button @button:click="showNewCustomerModal">
                Add Customer
            </solar-button>
        </div>
        <table id="customers" class="table">
            <tr>
                <th>Customer</th>
                <th>Address</th>
                <th>State</th>
                <th>Since</th>
                <th>Delete</th>
            </tr>
            <tr v-for="customer in customers" v-bind:key="customer.id">
                <td>{{customer.firstName + ' ' + customer.lastName}}</td>
                <td>{{customer.primaryAddress.addressLine1 + ' ' + customer.primaryAddress.addressLine2}}</td>
                <td>{{customer.primaryAddress.state}}</td>
                <td>{{customer.createdOn | humanizeDate}} </td>
                <td class="lni lni-trash-can customer-archive" @click="deleteCustomer(customer.id)"></td>
            </tr>
        </table>
        <new-customer-modal 
                            @close="closeModal"
                            @save:customer="saveNewCustomer"
                            v-if="isNewCustomerModalVisable">

        </new-customer-modal>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import SolarButton from '@/components/SolarButton.vue';
    import NewCustomerModal from '@/components/modals/NewCustomerModal.vue'
    import { ICustomer } from '@/types/Customer';
    import CustomerService from '@/services/customer-service';

    const customerService = new CustomerService();

    @Component({ name: 'Customers', components: { SolarButton, NewCustomerModal } })
    export default class Customers extends Vue {
        customers: ICustomer[] = [];
        isNewCustomerModalVisable = false;

        showNewCustomerModal() {
            this.isNewCustomerModalVisable = true;
        }

        closeModal() {
            this.isNewCustomerModalVisable = false;
        }

        async saveNewCustomer(newCustomer: ICustomer) {
            await customerService.addCustomer(newCustomer);
            this.isNewCustomerModalVisable = false;
            await this.initialize();
        }

        async deleteCustomer(id: number) {
            await customerService.deleteCustomer(id);
            await this.initialize();
        }

        async initialize() {
            this.customers = await customerService.getCustomers();
        }

        async created() {
            await this.initialize();
        }

    }
</script>

<style scoped lang="scss">
    @import '@/scss/global.scss';

    .customer-actions {
        display: flex;
        margin-bottom: 0.8rem;
    }

    .customer-archive {
        cursor: pointer;
        font-weight: bold;
        font-size: 1.2rem;
        color: $solar-red;
    }
</style>
<!--suppress XmlUnboundNsPrefix -->

<template>
	<solar-modal>
		<template v-slot:header>
			Add New Product
		</template>
		<template v-slot:body>
			<ul class="newProduct">
				<li>
					<label for="productName">Name</label>
					<input type="text" id="productName" v-model="newProduct.name" @change="validateName" />
				</li>

				<li>
					<label for="productDesc">Description</label>
					<input type="text"
						   id="productDesc"
						   v-model="newProduct.description" />
				</li>

				<li>
					<label for="productPrice">Price (USD)</label>
					<input type="number" id="productPrice" v-model="newProduct.price" />
				</li>
				<li>
					<label for="isTaxable" id="taxableLabel">Is this product taxable?</label>
					<input type="checkbox"
						   id="isTaxable"
						   v-model="newProduct.isTaxable" />
				</li>
			</ul>
			<div></div>
			<ul v-if="displayErrors">
				<li v-for="(error, index) in validationErrors" :key="index" class="red">
				{{error}}
				</li>
			</ul>
		</template>
		<template v-slot:footer>
			<solar-button type="button"
						  @click.native="save"
						  aria-label="save new item">
				Save Product
			</solar-button>
			<solar-button type="button"
						  @click.native="close"
						  aria-label="close modal">
				Close
			</solar-button>
		</template>
	</solar-modal>
</template>

<script lang="ts">
	import { Component, Prop, Vue } from "vue-property-decorator";
	import SolarButton from "@/components/SolarButton.vue";
	import SolarModal from "@/components/modals/SolarModal.vue";
	import { IProduct, IProductInventory } from "@/types/Product";
	@Component({
		name: "NewProductModal",
		components: { SolarButton, SolarModal }
	})
	export default class NewProductModal extends Vue {
		newProduct: IProduct = {
			createdOn: new Date(),
			updatedOn: new Date(),
			id: 0,
			description: "",
			isTaxable: false,
			name: "",
			price: 0,
			isArchived: false
		};
		validationErrors: string[] = [];
		displayErrors = false;

		close() {
			this.$emit("close");
		}
		save() {
			if (!this.validateProduct()) {
				this.displayErrors = true;
				return;
			} else {
				this.displayErrors = false;
                this.$emit("save:product", this.newProduct);
            }
			
		}
		validateProduct() {
            this.validationErrors = [];
			if (!this.validatePrice()) {
				this.validationErrors.push("You must enter a valid price.");
			}
			if (!this.validateName()) {
                this.validationErrors.push("You must enter a valid name.");
			}
			if (!this.validateDescription()) {
                this.validationErrors.push("You must enter a valid description.");
			}
			if (!this.validationErrors.length) {
				return true;
			} else {
                return false;
			}
			
        }
		validatePrice() {
			if (this.newProduct.price <= 0) {
				return false;
			}
			return true;
		}
		validateName() {
            if (!this.newProduct.name) {
				return false;
			}
			return true;
        }
		validateDescription() {
			if (!this.newProduct.description) {
                return false;
            }
            return true;
		}
		

	}
</script>

<style scoped lang="scss">
    @import '@/scss/global.scss';

    .red {
        font-weight: bold;
        color: $solar-red;
    }
	.newProduct {
		list-style: none;
		padding: 0;
		margin: 0;

		input {
		width: 100%;
		height: 1.8rem;
		margin-bottom: 1.2rem;
		font-size: 1.1rem;
		line-height: 1.3rem;
		padding: 0.2rem;
		color: #555;
	}
	

	label {
		font-weight: bold;
		display: block;
		margin-bottom: 0.3rem;
	}

	#taxableLabel {
		display: inline;
	}

    #isTaxable {
        position: relative;
        top: .5rem;
        width: 55px;
    }
	}
</style>
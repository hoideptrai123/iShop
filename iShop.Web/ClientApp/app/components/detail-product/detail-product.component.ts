import { Component, Output, EventEmitter,Input,OnInit } from '@angular/core';
//import { CookieService } from 'ngx-cookie-service';
import { Cart } from "../../model/Cart";
@Component({
    selector: 'detail-product',
    templateUrl: './detail-product.component.html',
    styleUrls: ['./detail-product.component.css']
})
export class DetailProductComponent implements OnInit {
    ngOnInit(): void {
        var currentCart = JSON.parse(String(localStorage.getItem(this.product.id)));
        if (currentCart) this.quantity = currentCart.quantity;
    }

    @Output() onclick = new EventEmitter<boolean>();
    @Input('product') product: any;
    
    max: number = 5;
    rate: number = 4;
    isReadonly: boolean = true;
    quantity: number = 1;

    constructor() {
     
        
    }

    addToCart() {
        this.onclick.emit(true);
        let cart: Cart = new Cart(this.product.id, this.quantity, this.product.price);
        localStorage.setItem(this.product.id, JSON.stringify(cart));
    }

    changeValue(isChange: boolean) {

        isChange ? this.quantity++ : this.quantity--;
    }


}

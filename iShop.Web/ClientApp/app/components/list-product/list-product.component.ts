﻿
import { ProductService } from '../../service/product.service';
import { Product } from "../../model/product";
import { ChangeDetectorRef, TemplateRef, Component, OnInit,Input } from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
@Component({
    selector: 'list-product',
    templateUrl: './list-product.component.html',
    styleUrls: ['./list-product.component.css']
})
export class ListProductComponent {
    @Input('name') name: any;
    @Input('title') title?: string;
    products: Product[]=[];
    bought:boolean=false;
    start: number = -1;
    end: number = 3;
    viewProduct: boolean = false;
    product?: Product;
    modalRef?: BsModalRef;


    constructor(private productService: ProductService) {
        this.productService.getProducts().subscribe(p => {
            this.products = p;
        } );
    }
  

   
    //next button
    next() {
        if (this.end < 10) {
            this.start += 2;
            this.end += 2;
            if (this.end === 9) {
                this.viewProduct = true;
            }
        }
    }
    //previous button
    pre() {
        if (this.start > 1) {
            this.start -= 2;
            this.end -= 2;
        }
    }

  
}

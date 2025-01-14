import { Component, inject, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Product } from '../../shared/models/product.model';
import { ShopService } from '../../core/services/shop.service';
import { ProductItemComponent } from './product-item/product-item.component';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { ShopParams } from '../../shared/models/shopParams';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination.model';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account.service';

type Sorting = { name: string; value: string };

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenuModule,
    MatListModule,
    MatPaginatorModule,
    FormsModule,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private matdialog = inject(MatDialog);

  shopParams = new ShopParams();

  products = signal<Pagination<Product> | null>(null);

  pageSizeOptions = [5, 10, 15, 20];

  options = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price : Low-High', value: 'priceAsc' },
    { name: 'Price : High-Low', value: 'priceDsc' },
  ];

  sortOptions = signal<Sorting[]>(this.options);

  ngOnInit(): void {
    this.initializeShop();
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
  }

  //Getting all the products from database
  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: (res) => {
        this.products?.set(res);
      },
      error: (err) => console.log(err),
    });
  }

  // Handling Search click event

  onSearchChange() {
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  //Handling Pagination

  handlePagechange(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();
  }

  // Handling sort click event

  onSortChange(event: MatSelectionListChange) {
    const selectedOptions = event.options[0];
    if (selectedOptions) {
      this.shopParams.sort = selectedOptions.value;
      this.shopParams.pageNumber = 1;
    }

    this.getProducts();
  }

  // Opening Dialog PopUp Component and passing the data
  //  and reciving the data on closing dialog

  openDialog() {
    const dialogRef = this.matdialog.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedTypes: this.shopParams.types,
        selectedBrands: this.shopParams.brands,
      },
    });

    dialogRef.afterClosed().subscribe({
      next: (res) => {
        if (res) {
          this.shopParams.brands = res.selectedBrands();
          this.shopParams.types = res.selectedTypes();
          this.shopParams.pageNumber = 1;

          this.getProducts();
        }
      },
    });
  }
}

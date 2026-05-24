import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicesEditComponent } from './invoices-edit.component';

describe('InvoicesEditComponent', () => {
  let component: InvoicesEditComponent;
  let fixture: ComponentFixture<InvoicesEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoicesEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoicesEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

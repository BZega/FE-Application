import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbilityCreatorComponent } from './ability-creator.component';

describe('AbilityCreatorComponent', () => {
  let component: AbilityCreatorComponent;
  let fixture: ComponentFixture<AbilityCreatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AbilityCreatorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AbilityCreatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

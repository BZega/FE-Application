import { Component, inject } from '@angular/core';
import { Character } from '../../core/models/Character';
import { Observable, Subject } from 'rxjs';
import { CharacterService } from '../../core/services/character.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material/material.module';
import { CharacterSummaryComponent } from '../../components/character-summary/character-summary.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CharacterCreatorComponent } from '../../components/character-creator/character-creator.component';
import { ClassCreatorComponent } from '../../components/class-creator/class-creator.component';
import { AbilityCreatorComponent } from '../../components/ability-creator/ability-creator.component';

@Component({
  selector: 'app-character-summary-container',
  imports: [CommonModule, MaterialModule, CharacterSummaryComponent, MatDialogModule],
  templateUrl: './character-summary-container.component.html',
  styleUrl: './character-summary-container.component.scss'
})
export class CharacterSummaryContainerComponent {
  private characterService = inject(CharacterService);
  private dialog = inject(MatDialog);
  
  trackCharactersBy(index: number, character: { id: string }): string {
    return character.id;
  }
  
  allCharacters: Character[] = [];
  characterList$: Observable<Character[]>;

  private ngUnsubscribe = new Subject();

  openCharacterCreator() {
    const dialogRef = this.dialog.open(CharacterCreatorComponent, {
      width: 'calc(95vw - 200px)',
      maxWidth: '1000px',
      height: '95vh',
      maxHeight: '900px',
      disableClose: false,
      panelClass: 'character-creator-dialog'
    });

    dialogRef.backdropClick().subscribe(() => {
      const component = dialogRef.componentInstance;
      component.cancelCreation();
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'created') {
        // Refresh the character list
        this.ngOnInit();
      }
    });
  }

  openClassCreator() {
    const dialogRef = this.dialog.open(ClassCreatorComponent, {
      width: 'calc(95vw - 200px)',
      maxWidth: '1200px',
      height: '95vh',
      maxHeight: '900px',
      disableClose: false,
      panelClass: 'class-creator-dialog'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Class created:', result);
        // Optionally refresh data if needed
      }
    });
  }

  openAbilityCreator() {
    const dialogRef = this.dialog.open(AbilityCreatorComponent, {
      width: '800px',
      maxWidth: '90vw',
      disableClose: false,
      panelClass: 'ability-creator-dialog'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Ability created:', result);
      }
    });
  }

  ngOnInit(): void {
    this.characterService.getAllCharacters().subscribe({
      next: (data) => {
        console.log("Received data:", data);
        this.allCharacters = data;
        this.characterService.characterList.next(this.allCharacters);
      },
      error: (error) => console.error("HTTP Error:", error),
      complete: () => console.log("Request complete"),
    })

    this.characterList$ = this.characterService.characterList$;
    console.log("Brandon - ", this.characterList$);
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next(null);
    this.ngUnsubscribe.complete();
  }
}

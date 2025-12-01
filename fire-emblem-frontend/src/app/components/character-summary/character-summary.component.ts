import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { Character } from '../../core/models/Character';
import { CharacterService } from '../../core/services/character.service';

@Component({
  selector: 'character-summary',
  imports: [CommonModule],
  templateUrl: './character-summary.component.html',
  styleUrl: './character-summary.component.scss'
})
export class CharacterSummaryComponent {
  @Input() character: Character;
  @Input() index: number;
  selectedCharacterNumber$: Observable<string | null>;
  
  
  constructor(
    private characterService: CharacterService,
    private router: Router
  ) { }
  
  ngOnInit(): void {
    this.selectedCharacterNumber$ = this.characterService.selectedCharacter$.pipe(
      map(character => character?.id ?? null)
    );
  }

  isSelected(): Observable<boolean> {
    return this.selectedCharacterNumber$.pipe(
      map(x => x === this.character.id)
    );
  }

  onSelect(event: Event) {
    event.stopPropagation();
    this.characterService.selectedCharacter.next(this.character);
    // Navigate to character detail page
    this.router.navigate(['/character-detail', this.character.id]);
  }
}

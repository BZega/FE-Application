import { Routes } from '@angular/router';
import { CharacterSummaryContainerComponent } from './container/character-summary-container/character-summary-container.component';
import { CharacterCreatorComponent } from './components/character-creator/character-creator.component';

export const routes: Routes = [
    { path: 'character-summary-container', component: CharacterSummaryContainerComponent, title: "Character Summary Page" },
    { path: 'character-creator', component: CharacterCreatorComponent, title: "Character Creation Page" },
    { path: '**', redirectTo: '/character-summary-container', pathMatch: 'full' }
];

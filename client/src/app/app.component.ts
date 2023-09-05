import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User} from './_models/user'
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Dating app';
  users:any;

  constructor(private https: HttpClient, private accountService:AccountService){}
  ngOnInit() {
    this.getUsers();
    
    this.setCurrentUser();
      
  }
  getUsers(){
    this.https.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users.username,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    })
  }
  setCurrentUser()
  {
    const user:User=JSON.parse(localStorage.getItem('user')!);
    this.accountService.setCurrentUser(user);
  }
 
    
}
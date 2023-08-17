var app = new Vue({
    el:"#app",
    data:{
        clientInfo: {},
        accounts: [],
        credits: [],
        //error: null
        errorToats: null,
        errorMsg: null,
    },
    methods:{
        getData: function(){
            //axios.get("/api/clients/1")
            axios.get("/api/clients/current")
            .then(function (response) {
                //get client ifo
                app.clientInfo = response.data;
                console.log('data: ',response.data)
                app.accounts = response.data.accounts.$values;
                app.credits = response.data.credits.$values;
                console.log('accounts: ',app.accounts)
                console.log('credits: ',app.credits)
            })
            .catch(function (error) {
                // handle error
                //app.error = error;
                this.errorMsg = "Error getting data";
                console.log('Error: ',error)
            })
        },
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        },
        signOut: function () {
            axios.post('/api/auth/logout')
                .then(response => window.location.href = "/index.html")
                .catch(() => {
                    this.errorMsg = "Sign out failed"
                    this.errorToats.show();
                })
        },
        create: function(){
            axios.post('/api/clients/current/accounts')
            .then(response => window.location.reload())
            .catch((error) =>{
                this.errorMsg = error.response.data;  
                this.errorToats.show();
            })
        }        
    },
    mounted: function () {
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
        this.getData();
    }
})
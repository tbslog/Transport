import axios from "axios";
import { ToastSuccess, ToastError } from "../Common/FuncToast";
import Cookies from "js-cookie";
import jwt_decode from "jwt-decode";
import { Buffer } from "buffer";

//const Host = "https://api.tbslogistics.com.vn/api/";
//const Host = "http://192.168.0.109:8088/api/";
const Host = "https://localhost:5001/api/";

axios.interceptors.request.use(
  (config) => {
    let tokens = Cookies.get("token");
    if (tokens && tokens.length > 0) {
      const decoded = jwt_decode(tokens);
      const exp = decoded.exp;
      const expired = Date.now() >= exp * 1000;
      if (expired) {
        Object.keys(Cookies.get()).forEach(function (cookieName) {
          var neededAttributes = {
            // Here you pass the same attributes that were used when the cookie was created
            // and are required when removing the cookie
          };
          Cookies.remove(cookieName);
        });
        ToastError("Phiên đăng nhập đã hết hạn");
        return window.location.reload();
      }
      config.headers["Authorization"] = `Bearer ${tokens}`;
      return config;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axios.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    if (
      error.response.status === 401 ||
      error.response.statusText === "Unauthorized"
    ) {
      // handle error: inform user, go to login, etc
      Object.keys(Cookies.get()).forEach(function (cookieName) {
        var neededAttributes = {
          // Here you pass the same attributes that were used when the cookie was created
          // and are required when removing the cookie
        };
        Cookies.remove(cookieName);
      });
      ToastError("Phiên đăng nhập đã hết hạn");
      return window.location.reload();
    } else {
      return Promise.reject(error);
    }
  }
);

const getData = async (url) => {
  let data;
  await axios
    .get(Host + url)
    .then((response) => {
      data = response.data;
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
    });

  return data;
};

const postLogin = async (url, data, header = null) => {
  var result = { isSuccess: 0, Message: "" };
  await axios
    .post(Host + url, data, header)
    .then((response) => {
      Cookies.set("token", response.data);
      return (result = { isSuccess: 1, Message: "Đăng nhập thành công!" });
    })
    .catch((error) => {
      return (result = { isSuccess: 0, Message: error.response.data });
    });

  return result;
};

const getDataCustom = async (url, data, header = null) => {
  let dataReturn = [];
  await axios
    .post(Host + url, data, header)
    .then((response) => {
      dataReturn = response.data;
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
    });
  return dataReturn;
};

const getFile = async (url, fileName) => {
  axios
    .get(Host + url, {
      responseType: "arraybuffer",
    })
    .then((response) => {
      let blob = new Blob([response.data], {
          type: `${response.headers["content-type"]}`,
        }),
        url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${fileName}`);
      document.body.appendChild(link);
      link.click();
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
    });
};

const getFilePost = async (url, data, fileName) => {
  axios
    .post(Host + url, data, {
      responseType: "arraybuffer",
    })
    .then((response) => {
      let blob = new Blob([response.data], {
          type: `${response.headers["content-type"]}`,
        }),
        url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${fileName}`);
      document.body.appendChild(link);
      link.click();
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
    });
};

const getFileImage = async (url) => {
  let src = "";
  await axios
    .get(
      Host + url,
      {
        responseType: "arraybuffer",
      },
      {
        headers: { accept: "*/*", "Content-Type": "multipart/form-data" },
      }
    )
    .then((response) => {
      // let base64string = btoa(
      //   String.fromCharCode(...new Uint8Array(response.data))
      // );
      let base64string = Buffer.from(response.data).toString("base64");

      let contentType = response.headers["content-type"];
      src = "data:" + contentType + ";base64," + base64string;
      return src;
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
    });

  return src;
};

const postData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .post(Host + url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const postFile = async (url, data) => {
  var isSuccess = 0;

  await axios
    .post(Host + url, data, {
      headers: { accept: "*/*", "Content-Type": "multipart/form-data" },
    })
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });
  return isSuccess;
};

const putData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .put(Host + url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const deleteData = async (url) => {
  var isSuccess = 0;
  axios.delete(Host + url).then(
    (response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    },
    (error) => {
      ToastSuccess(`${error.response.data}`);
      return (isSuccess = 0);
    }
  );
  return isSuccess;
};

export {
  getData,
  postData,
  putData,
  deleteData,
  postFile,
  getFile,
  getDataCustom,
  getFileImage,
  getFilePost,
  postLogin,
};

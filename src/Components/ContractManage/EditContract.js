import { useState, useEffect, useCallback } from "react";
import { getData, postData, getFile } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import moment from "moment";
import DatePicker from "react-datepicker";

const EditContract = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    setValue,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 20,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TenHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value:
          /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaKh: {
      required: "Không được để trống",
      maxLength: {
        value: 8,
        message: "Không được vượt quá 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Không được ít hơn 8 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    PhanLoaiHopDong: {
      required: "Không được để trống",
    },
    SoHopDongCha: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    NgayBatDau: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pettern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    NgayKetThuc: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pettern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    PTVC: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
    PhuPhi: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
  };

  const [isContract, setIsContract] = useState(true);
  const [listContractType, setListContractType] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listStatusType, setListStatusType] = useState([]);

  const [downloadFile, setDownloadFile] = useState();

  useEffect(() => {
    if (
      props &&
      props.selectIdClick &&
      Object.keys(props.selectIdClick).length > 0 &&
      Object.keys(props).length > 0
    ) {
      SetIsLoading(true);
      setTimeout(() => {
        if (props.selectIdClick.soHopDongCha) {
          setIsContract(false);
          setValue("SoHopDongCha", props.selectIdClick.soHopDongCha);
          setValue("PhuPhi", props.selectIdClick.phuPhi);
        } else {
          setIsContract(true);
        }
        setValue("MaHopDong", props.selectIdClick.maHopDong);
        setValue("TenHopDong", props.selectIdClick.tenHienThi);
        setValue("MaKh", props.selectIdClick.maKh);
        setValue("GhiChu", props.selectIdClick.ghiChu);
        setDownloadFile(props.selectIdClick.file);
        setValue("TrangThai", props.selectIdClick.trangThai);
        setValue("NgayBatDau", new Date(props.selectIdClick.thoiGianBatDau));
        setValue("NgayKetThuc", new Date(props.selectIdClick.thoiGianKetThuc));
        SetIsLoading(false);
      }, 1000);
    }
  }, [props, props.selectIdClick, setValue]);

  useEffect(() => {
    if (props && props.listContractType) {
      setListContractType(props.listContractType);
    }
  }, [props, props.listContractType]);

  useEffect(() => {
    SetIsLoading(true);
    setListStatusType(props.listStatus);
    SetIsLoading(false);
  }, []);

  const handleDownloadContact = async () => {
    SetIsLoading(true);
    const getFileDownLoad = getFile(
      `Contract/DownloadFile?fileId=${downloadFile}`,
      watch("TenHopDong")
    );
    SetIsLoading(false);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    var update = await postData(
      `Contract/UpdateContract?Id=${data.MaHopDong}`,
      {
        tenHienThi: data.TenHopDong,
        thoiGianBatDau: moment(new Date(data.NgayBatDau).toISOString()).format(
          "YYYY-MM-DD"
        ),
        thoiGianKetThuc: moment(
          new Date(data.NgayKetThuc).toISOString()
        ).format("YYYY-MM-DD"),
        ghiChu: data.GhiChu,
        phuPhi: isContract === false ? data.PhuPhi : null,
        trangThai: data.TrangThai,
        file: data.FileContact[0],
      },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );

    if (update === 1) {
      props.getListContract(1);
      props.hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      {isContract === true && (
        <>
          <div className="card card-primary">
            <div className="card-header">
              <h3 className="card-title">Form Cập Nhật Hợp Đồng</h3>
            </div>
            <div>{IsLoading === true && <div>Loading...</div>}</div>

            {IsLoading === false && (
              <form onSubmit={handleSubmit(onSubmit)}>
                <div className="card-body">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaHopDong">Mã Hợp Đồng</label>
                        <input
                          readOnly
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="MaHopDong"
                          placeholder="Nhập mã hợp đồng"
                          {...register("MaHopDong", Validate.MaHopDong)}
                        />
                        {errors.MaHopDong && (
                          <span className="text-danger">
                            {errors.MaHopDong.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TenHopDong">Tên Hợp Đồng</label>
                        <input
                          type="text"
                          className="form-control"
                          id="TenHopDong"
                          placeholder="Nhập tên khách hàng"
                          {...register("TenHopDong", Validate.TenHopDong)}
                        />
                        {errors.TenHopDong && (
                          <span className="text-danger">
                            {errors.TenHopDong.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaKh">Mã Khách Hàng</label>
                        <input
                          type="text "
                          className="form-control"
                          id="MaKh"
                          placeholder="Nhập mã khách hàng"
                          readOnly
                          {...register("MaKh", Validate.MaKh)}
                        />
                        {errors.MaKh && (
                          <span className="text-danger">
                            {errors.MaKh.message}
                          </span>
                        )}
                      </div>
                    </div>
                    {/* <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="PhanLoaiHopDong">
                          Phân Loại Hợp Đồng
                        </label>
                        <select
                          className="form-control"
                          {...register(
                            "PhanLoaiHopDong",
                            Validate.PhanLoaiHopDong
                          )}
                        >
                          <option value="">Chọn phân loại HĐ</option>
                          {listContractType &&
                            listContractType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiHopDong}
                                  key={val.maLoaiHopDong}
                                >
                                  {val.tenLoaiHopDong}
                                </option>
                              );
                            })}
                        </select>
                        {errors.PhanLoaiHopDong && (
                          <span className="text-danger">
                            {errors.PhanLoaiHopDong.message}
                          </span>
                        )}
                      </div>
                    </div> */}
                    {/* <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="PTVC">Phương thức vận chuyển</label>
                        <select
                          className="form-control"
                          {...register("PTVC", Validate.PTVC)}
                        >
                          <option value="">Chọn phương thức vận chuyển</option>
                          {listTransportType &&
                            listTransportType.map((val) => {
                              return (
                                <option value={val.maPtvc} key={val.maPtvc}>
                                  {val.tenPtvc}
                                </option>
                              );
                            })}
                        </select>
                        {errors.PTVC && (
                          <span className="text-danger">
                            {errors.PTVC.message}
                          </span>
                        )}
                      </div>
                    </div> */}
                  </div>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="NgayBatDau">Ngày bắt đầu</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name="NgayBatDau"
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                dateFormat="dd/MM/yyyy"
                                placeholderText="Chọn ngày bắt đầu"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                            rules={Validate.NgayBatDau}
                          />
                          {errors.NgayBatDau && (
                            <span className="text-danger">
                              {errors.NgayBatDau.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="NgayKetThuc">Ngày kết thúc</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name="NgayKetThuc"
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                dateFormat="dd/MM/yyyy"
                                placeholderText="Chọn ngày bắt đầu"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                            rules={Validate.NgayKetThuc}
                          />
                          {errors.NgayKetThuc && (
                            <span className="text-danger">
                              {errors.NgayKetThuc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>

                  <div className="form-group">
                    <label htmlFor="GhiChu">Ghi Chú</label>
                    <input
                      type="text"
                      className="form-control"
                      id="GhiChu"
                      placeholder="Nhập ghi chú"
                      {...register("GhiChu")}
                    />
                    {errors.GhiChu && (
                      <span className="text-danger">
                        {errors.GhiChu.message}
                      </span>
                    )}
                  </div>

                  <div className="form-group">
                    <label htmlFor="FileContact">Tải lên tệp Hợp Đồng</label>
                    <input
                      type="file"
                      className="form-control-file"
                      {...register("FileContact", Validate.FileContact)}
                    />
                    {errors.FileContact && (
                      <span className="text-danger">
                        {errors.FileContact.message}
                      </span>
                    )}
                  </div>

                  {downloadFile && downloadFile !== "0" && (
                    <div>
                      <div className="form-group">
                        <label htmlFor="FileContact">Tải về tệp Hợp Đồng</label>
                        <br />
                        <button
                          type="button"
                          className="btn btn-default"
                          onClick={() => handleDownloadContact()}
                        >
                          <i className="fas fa-file-download">
                            {" "}
                            Tải tệp hợp đồng
                          </i>
                        </button>
                      </div>
                    </div>
                  )}

                  <div className="form-group">
                    <label htmlFor="TrangThai">Trạng thái</label>
                    <select
                      className="form-control"
                      {...register("TrangThai", Validate.TrangThai)}
                    >
                      <option value="">Chọn trạng thái</option>
                      {listStatusType &&
                        listStatusType.map((val) => {
                          return (
                            <option value={val.statusId} key={val.statusId}>
                              {val.statusContent}
                            </option>
                          );
                        })}
                    </select>
                    {errors.TrangThai && (
                      <span className="text-danger">
                        {errors.TrangThai.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="card-footer">
                  <div>
                    <button
                      type="submit"
                      className="btn btn-primary"
                      style={{ float: "right" }}
                    >
                      Cập nhật
                    </button>
                  </div>
                </div>
              </form>
            )}
          </div>
        </>
      )}
      {isContract === false && (
        <>
          <div className="card card-primary">
            <div className="card-header">
              <h3 className="card-title">Form Cập Nhật Phục Lục Hợp Đồng</h3>
            </div>
            <div>{IsLoading === true && <div>Loading...</div>}</div>

            {IsLoading === false && (
              <form onSubmit={handleSubmit(onSubmit)}>
                <div className="card-body">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaHopDong">Mã Hợp Đồng</label>
                        <input
                          readOnly
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="MaHopDong"
                          placeholder="Nhập mã hợp đồng"
                          {...register("MaHopDong", Validate.MaHopDong)}
                        />
                        {errors.MaHopDong && (
                          <span className="text-danger">
                            {errors.MaHopDong.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="TenHopDong">Tên Hợp Đồng</label>
                        <input
                          type="text"
                          className="form-control"
                          id="TenHopDong"
                          placeholder="Nhập tên khách hàng"
                          {...register("TenHopDong", Validate.TenHopDong)}
                        />
                        {errors.TenHopDong && (
                          <span className="text-danger">
                            {errors.TenHopDong.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaKh">Mã Khách Hàng</label>
                        <input
                          readOnly
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="MaKh"
                          placeholder="Nhập mã hợp đồng"
                          {...register("MaKh", Validate.MaKh)}
                        />
                        {errors.MaKh && (
                          <span className="text-danger">
                            {errors.MaKh.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="SoHopDongCha">Số hợp đồng cha</label>
                        <input
                          readOnly
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="SoHopDongCha"
                          placeholder="Nhập mã hợp đồng"
                          {...register("SoHopDongCha", Validate.SoHopDongCha)}
                        />
                        {errors.SoHopDongCha && (
                          <span className="text-danger">
                            {errors.SoHopDongCha.message}
                          </span>
                        )}
                      </div>
                    </div>
                    {/* <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="PhanLoaiHopDong">
                          Phân Loại Hợp Đồng
                        </label>
                        <select
                          className="form-control"
                          {...register(
                            "PhanLoaiHopDong",
                            Validate.PhanLoaiHopDong
                          )}
                        >
                          <option value="">Chọn phân loại HĐ</option>
                          {listContractType &&
                            listContractType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiHopDong}
                                  key={val.maLoaiHopDong}
                                >
                                  {val.tenLoaiHopDong}
                                </option>
                              );
                            })}
                        </select>
                        {errors.PhanLoaiHopDong && (
                          <span className="text-danger">
                            {errors.PhanLoaiHopDong.message}
                          </span>
                        )}
                      </div>
                    </div> */}
                    {/* <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="PTVC">Phương thức vận chuyển</label>
                        <select
                          className="form-control"
                          {...register("PTVC", Validate.PTVC)}
                        >
                          <option value="">Chọn phương thức vận chuyển</option>
                          {listTransportType &&
                            listTransportType.map((val) => {
                              return (
                                <option value={val.maPtvc} key={val.maPtvc}>
                                  {val.tenPtvc}
                                </option>
                              );
                            })}
                        </select>
                        {errors.PTVC && (
                          <span className="text-danger">
                            {errors.PTVC.message}
                          </span>
                        )}
                      </div>
                    </div> */}
                  </div>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="NgayBatDau">Ngày bắt đầu</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name="NgayBatDau"
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                dateFormat="dd/MM/yyyy"
                                placeholderText="Chọn ngày bắt đầu"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                            rules={Validate.NgayBatDau}
                          />
                          {errors.NgayBatDau && (
                            <span className="text-danger">
                              {errors.NgayBatDau.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="NgayKetThuc">Ngày kết thúc</label>
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name="NgayKetThuc"
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                dateFormat="dd/MM/yyyy"
                                placeholderText="Chọn ngày bắt đầu"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                            rules={Validate.NgayKetThuc}
                          />
                          {errors.NgayKetThuc && (
                            <span className="text-danger">
                              {errors.NgayKetThuc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="form-group">
                    <label htmlFor="PhuPhi">Phụ Phí</label>
                    <input
                      type="text"
                      className="form-control"
                      id="PhuPhi"
                      placeholder="Nhập phụ phí"
                      {...register("PhuPhi", Validate.PhuPhi)}
                    />
                    {errors.PhuPhi && (
                      <span className="text-danger">
                        {errors.PhuPhi.message}
                      </span>
                    )}
                  </div>
                  <div className="form-group">
                    <label htmlFor="GhiChu">Ghi Chú</label>
                    <input
                      type="text"
                      className="form-control"
                      id="GhiChu"
                      placeholder="Nhập ghi chú"
                      {...register("GhiChu")}
                    />
                    {errors.GhiChu && (
                      <span className="text-danger">
                        {errors.GhiChu.message}
                      </span>
                    )}
                  </div>

                  <div className="form-group">
                    <label htmlFor="FileContact">Tải lên tệp Hợp Đồng</label>
                    <input
                      type="file"
                      className="form-control-file"
                      {...register("FileContact", Validate.FileContact)}
                    />
                    {errors.FileContact && (
                      <span className="text-danger">
                        {errors.FileContact.message}
                      </span>
                    )}

                    {downloadFile && downloadFile !== "0" && (
                      <div>
                        <div className="form-group">
                          <label htmlFor="FileContact">
                            Tải về tệp Hợp Đồng
                          </label>
                          <br />
                          <button
                            type="button"
                            className="btn btn-default"
                            onClick={() => handleDownloadContact()}
                          >
                            <i className="fas fa-file-download">
                              Tải tệp hợp đồng
                            </i>
                          </button>
                        </div>
                      </div>
                    )}
                  </div>

                  <div className="form-group">
                    <label htmlFor="TrangThai">Trạng thái</label>
                    <select
                      className="form-control"
                      {...register("TrangThai", Validate.TrangThai)}
                    >
                      <option value="">Chọn trạng thái</option>
                      {listStatusType &&
                        listStatusType.map((val) => {
                          return (
                            <option value={val.statusId} key={val.statusId}>
                              {val.statusContent}
                            </option>
                          );
                        })}
                    </select>
                    {errors.TrangThai && (
                      <span className="text-danger">
                        {errors.TrangThai.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="card-footer">
                  <div>
                    <button
                      type="submit"
                      className="btn btn-primary"
                      style={{ float: "right" }}
                    >
                      Cập nhật
                    </button>
                  </div>
                </div>
              </form>
            )}
          </div>
        </>
      )}
    </>
  );
};

export default EditContract;
